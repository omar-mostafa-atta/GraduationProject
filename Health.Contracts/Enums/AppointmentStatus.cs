using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Contracts.Enums
{
    public enum AppointmentStatus
    {
        Pending,           // المريض حجز __الدك لسه مردش
        Confirmed,         // الدك وافق
        Rejected,          // الدك رفض
        Completed,         // الميعاد خلص
        CancelledPatient,  // المريض لغى
        CancelledDoctor,   // الدك لغى
        Rescheduled        // المريض بيغير مواعيد
    }
}