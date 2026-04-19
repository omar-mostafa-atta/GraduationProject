using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Calendy;
using Health.Contracts.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Health.Application.Services
{
    public class CalendlyService : ICalendlyService
    {
        private readonly WateenDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public CalendlyService(WateenDbContext dbContext,
            IHttpClientFactory httpClientFactory,
            IConfiguration config)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public string GetAuthorizationUrl(string doctorUserId)
        {
            // Encode the userId into the state parameter
            var state = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(doctorUserId));

            return $"https://auth.calendly.com/oauth/authorize" +
                   $"?client_id={_config["Calendly:ClientId"]}" +
                   $"&response_type=code" +
                   $"&redirect_uri={Uri.EscapeDataString(_config["Calendly:RedirectUri"])}" +
                   $"&state={state}";
        }

        // 2. Exchange code → tokens, then save to Doctor
        public async Task<bool> ConnectDoctorAsync(string doctorUserId, string code)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.User.Id == userGuid)
                ?? throw new Exception("Doctor not found.");

            // Exchange code for tokens
            var client = _httpClientFactory.CreateClient();
            var tokenResponse = await client.PostAsync(
                _config["Calendly:AuthUrl"],
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type",    "authorization_code" },
                    { "client_id",     _config["Calendly:ClientId"] },
                    { "client_secret", _config["Calendly:ClientSecret"] },
                    { "redirect_uri",  _config["Calendly:RedirectUri"] },
                    { "code",          code }
                })
            );

            if (!tokenResponse.IsSuccessStatusCode)
                throw new Exception("Failed to exchange code for Calendly tokens.");

            var tokens = await tokenResponse.Content
                .ReadFromJsonAsync<CalendlyTokenResponse>();

            // Fetch the doctor's Calendly user info
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

            var userResponse = await client.GetFromJsonAsync<CalendlyUserResponse>(
                $"{_config["Calendly:BaseApiUrl"]}/users/me");

            // Save to DB
            doctor.CalendlyAccessToken = tokens.AccessToken;
            doctor.CalendlyRefreshToken = tokens.RefreshToken;
            doctor.CalendlyUri = userResponse!.Resource.Uri;
            doctor.CalendlySchedulingUrl = userResponse.Resource.SchedulingUrl;
            doctor.CalendlyOrganizationUri = userResponse.Resource.CurrentOrganization;

            await _dbContext.SaveChangesAsync();

            // Register webhook so we get notified when patients book
            await RegisterWebhookAsync(doctor, tokens.AccessToken);

            return true;
        }

        // 3. Get doctor's Calendly event types (consultation types)
        public async Task<List<CalendlyEventType>> GetDoctorEventTypesAsync(Guid doctorId)
        {
            var doctor = await _dbContext.Doctors.FindAsync(doctorId)
                ?? throw new Exception("Doctor not found.");

            if (!doctor.IsCalendlyConnected)
                throw new Exception("Doctor has not connected Calendly.");

            var client = GetAuthorizedClient(doctor.CalendlyAccessToken!);

            var response = await client.GetFromJsonAsync<CalendlyEventTypesResponse>(
                $"{_config["Calendly:BaseApiUrl"]}/event_types?user={Uri.EscapeDataString(doctor.CalendlyUri!)}&active=true");

            return response!.Collection;
        }

        //4. Get available time slots for a specific event type
        public async Task<List<AvailableSlot>> GetAvailableSlotsAsync(
            Guid doctorId, string eventTypeUri, DateTime from, DateTime to)
        {
            var doctor = await _dbContext.Doctors.FindAsync(doctorId)
                ?? throw new Exception("Doctor not found.");

            var client = GetAuthorizedClient(doctor.CalendlyAccessToken!);

            var url = $"{_config["Calendly:BaseApiUrl"]}/event_type_available_times" +
                      $"?event_type={Uri.EscapeDataString(eventTypeUri)}" +
                      $"&start_time={from:o}" +
                      $"&end_time={to:o}";

            var response = await client.GetFromJsonAsync<AvailableTimesResponse>(url);

            return response!.Collection.Select(t => new AvailableSlot
            {
                StartTime = t.StartTime,
                EndTime = t.StartTime.AddMinutes(t.InviteesRemaining > 0 ? 30 : 0),
                Status = t.Status
            }).ToList();
        }

        //5. Handle incoming webhook from Calendly
        public async Task ProcessWebhookAsync(string body, string signature)
        {
            // Verify it's really from Calendly
            if (!VerifySignature(body, signature))
               throw new UnauthorizedAccessException("Invalid Calendly webhook signature.");

            var payload = JsonSerializer.Deserialize<CalendlyWebhookPayload>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null) return;

            switch (payload.Event)
            {
                case "invitee.created":
                    await HandleBookingCreatedAsync(payload.Payload);
                    break;
                case "invitee.canceled":
                    await HandleBookingCancelledAsync(payload.Payload);
                    break;
            }
        }

        //  De Helper Functions ya shrook

        //private async Task HandleBookingCreatedAsync(CalendlyInviteePayload payload)
        //{
        //    // Find doctor by Calendly URI
        //    var doctorCalendlyUri = payload.Event.EventMemberships.FirstOrDefault()?.UserUri;
        //    var doctor = await _dbContext.Doctors
        //        .Include(d => d.User)
        //        .FirstOrDefaultAsync(d => d.CalendlyUri == doctorCalendlyUri);

        //    if (doctor == null) return;

        //    // Find patient by email
        //    var patient = await _dbContext.Patients
        //        .Include(p => p.User)
        //        .FirstOrDefaultAsync(p => p.User.Email == payload.Email);

        //    if (patient == null) return;

        //    // Avoid duplicates
        //    var exists = await _dbContext.Appointments
        //        .AnyAsync(a => a.CalendlyEventUri == payload.Event.Uri);
        //    if (exists) return;

        //    var appointment = new Appointment
        //    {
        //        Id = Guid.NewGuid(),
        //        PatientId = patient.Id,
        //        DoctorId = doctor.Id,
        //        AppointmentTime = payload.Event.StartTime,
        //        Type = "video",           // default; customize as needed
        //        Status = AppointmentStatus.Confirmed, // Calendly = already confirmed
        //        Notes = payload.QuestionsAndAnswers
        //                                .FirstOrDefault()?.Answer,
        //        CalendlyEventUri = payload.Event.Uri,
        //        CalendlyJoinUrl = payload.Event.Location?.JoinUrl,
        //        VideoCallLink = payload.Event.Location?.JoinUrl,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    _dbContext.Appointments.Add(appointment);
        //    await _dbContext.SaveChangesAsync();
        //}
        private async Task HandleBookingCreatedAsync(CalendlyInviteePayload payload)
        {
            Console.WriteLine($"=== HANDLE BOOKING CREATED ===");
            Console.WriteLine($"Patient email from Calendly: {payload.Email}");

            // Use ScheduledEvent here!
            var doctorCalendlyUri = payload.ScheduledEvent.EventMemberships.FirstOrDefault()?.UserUri;
            Console.WriteLine($"Doctor Calendly URI: {doctorCalendlyUri}");

            var doctor = await _dbContext.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.CalendlyUri == doctorCalendlyUri);

            Console.WriteLine($"Doctor found: {doctor != null}");
            if (doctor == null) return;

            var patient = await _dbContext.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.User.Email == payload.Email);

            Console.WriteLine($"Patient found: {patient != null}");
            if (patient == null) return;

            var exists = await _dbContext.Appointments
                .AnyAsync(a => a.CalendlyEventUri == payload.ScheduledEvent.Uri);

            Console.WriteLine($"Appointment already exists: {exists}");
            if (exists) return;

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                AppointmentTime = payload.ScheduledEvent.StartTime,
                Type = "video",
                Status = AppointmentStatus.Confirmed,
                Notes = payload.QuestionsAndAnswers.FirstOrDefault()?.Answer,
                CalendlyEventUri = payload.ScheduledEvent.Uri,
                CalendlyJoinUrl = payload.ScheduledEvent.Location?.JoinUrl,
                VideoCallLink = payload.ScheduledEvent.Location?.JoinUrl,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Appointments.Add(appointment);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine($"=== APPOINTMENT SAVED SUCCESSFULLY ===");
        }

        private async Task HandleBookingCancelledAsync(CalendlyInviteePayload payload)
        {
            var appointment = await _dbContext.Appointments
        .FirstOrDefaultAsync(a => a.CalendlyEventUri == payload.EventUri);

            if (appointment == null) return;

            appointment.Status = AppointmentStatus.CancelledPatient;
            await _dbContext.SaveChangesAsync();
        }

        private async Task RegisterWebhookAsync(Doctor doctor, string accessToken)
        {
            var client = GetAuthorizedClient(accessToken);

            var payload = new
            {
                url = _config["Calendly:RedirectUri"]
                                   .Replace("/callback", "/webhook"),
                events = new[] { "invitee.created", "invitee.canceled" },
                organization = doctor.CalendlyOrganizationUri,
                user = doctor.CalendlyUri,
                scope = "user",
                signing_key = _config["Calendly:WebhookSigningKey"]
            };

            await client.PostAsJsonAsync(
                $"{_config["Calendly:BaseApiUrl"]}/webhook_subscriptions", payload);
        }

        private bool VerifySignature(string body, string signatureHeader)
        {
          
            var parts = signatureHeader.Split(',');
            if (parts.Length < 2) return false;

            var t = parts[0].Replace("t=", "").Trim();
            var signature = parts[1].Replace("v1=", "").Trim();

            if (string.IsNullOrEmpty(t) || string.IsNullOrEmpty(signature))
                return false;

            // Replay attack protection reject webhooks older than 3 minutes
            var webhookTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(t));
            if (DateTimeOffset.UtcNow - webhookTime > TimeSpan.FromMinutes(3))
                throw new Exception("Webhook rejected: timestamp outside tolerance zone.");

            //Signed payload = timestamp + "." + body  (exactly as Calendly docs say)
            var signedPayload = $"{t}.{body}";
            var key = Encoding.UTF8.GetBytes(_config["Calendly:WebhookSigningKey"]!);
            var payloadBytes = Encoding.UTF8.GetBytes(signedPayload);

            var expectedHash = HMACSHA256.HashData(key, payloadBytes);
            var expectedSig = Convert.ToHexString(expectedHash).ToLower();

            return expectedSig == signature.ToLower();
        }

        private HttpClient GetAuthorizedClient(string accessToken)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
            return client;
        }
    }

}