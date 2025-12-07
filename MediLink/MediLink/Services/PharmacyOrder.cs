using MediLink.Data;

namespace MediLink.Services
{
    /// <summary>
    /// Represents an order for prescription fulfillment at the in-house pharmacy.
    /// Demonstrates data propagation and lack of validation.
    /// 
    /// AVR Calculation:
    /// - Total Attributes: 3
    /// - Vulnerable Attributes: 0 (assuming order details aren't strictly PII, but context matters)
    /// - Actually, if it copies Patient data, it's vulnerable.
    /// </summary>
    public class PharmacyOrder
    {
        public Guid OrderID { get; set; }
        public Prescription Prescription { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "Pending";
        
        // VULNERABLE: Duplicated patient name for "efficiency"
        public string PatientName { get; set; } = string.Empty;

        public PharmacyOrder(Prescription prescription, PatientRecord patient)
        {
            OrderID = Guid.NewGuid();
            Prescription = prescription;
            OrderDate = DateTime.Now;
            
            // Duplicating data again!
            PatientName = patient.FullName;
        }

        public void MarkFulfilled()
        {
            Status = "Fulfilled";
            Console.WriteLine($"Order {OrderID} for {PatientName} marked as FULFILLED.");
        }
    }
}
