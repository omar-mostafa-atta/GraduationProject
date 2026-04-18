using Health.Contracts.Calendy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.IServices
{
    public interface ICalendlyService
    {
        Task<bool> ConnectDoctorAsync(string doctorUserId, string code);
        Task<List<CalendlyEventType>> GetDoctorEventTypesAsync(Guid doctorId);
        Task<List<AvailableSlot>> GetAvailableSlotsAsync(Guid doctorId, string eventTypeUri, DateTime from, DateTime to);
        Task ProcessWebhookAsync(string body, string signature);
        string GetAuthorizationUrl(string doctorUserId);
    }
}
