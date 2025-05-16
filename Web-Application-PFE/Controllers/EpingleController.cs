using Microsoft.AspNetCore.Mvc;
using Web_Application_PFE.Data;
using Web_Application_PFE.Models;
using Web_Application_PFE.ViewModels;

namespace Web_Application_PFE.Controllers
{
    public class EpingleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EpingleController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EnregistrerEpingle(AddRFQViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Epingle = new Epingle
                {
                    Customer = model.Customer,
                    Sales = model.Sales,
                    ClientId = model.ClientId,
                    RFQId = model.RFQId,
                    QuoteName = model.QuoteName,
                    MarketSegment = model.MarketSegment,
                    NumberOfRefToBeQuoted = model.NumberOfRefToBeQuoted,
                    MaxVolume = model.MaxVolume,
                    EstimatedVolume = model.EstimatedVolume,
                    SOPDate = model.SOPDate,
                    KOdate = model.KOdate,
                    CustomerDatadate = model.CustomerDatadate,
                    MaterialLeader = model.MaterialLeader,
                    MaterialDueDate = model.MaterialDueDate,
                    MaterialRelease = model.MaterialRelease,
                    TestLeader = model.TestLeader,
                    TestDueDate = model.TestDueDate,
                    TestRelease = model.TestRelease,
                    VALeader = model.VALeader,
                    LabourDueDate = model.LabourDueDate,
                    LabourRelease = model.LabourRelease,
                    CustomerDueDate = model.CustomerDueDate,
                    WorkingStatus = model.WorkingStatus,
                    ApprovalDate = model.ApprovalDate,
                    FeedbackDate = model.FeedbackDate,
                    Comments = model.Comments,
                    Statut = model.Statut,
                    Feasibilityassessment = model.Feasibilityassessment,
                    DFM = model.DFM,
                    DFT = model.DFT,
                    MaxRevenue = model.MaxRevenue,
                    EstimatedRevenue = model.EstimatedRevenue,
                    NRE = model.NRE,
                    StatutRFQ = model.StatutRFQ,
                    TimeRFQ = model.TimeRFQ,
                    DateCreation = DateTime.Now
                };

                _context.Epingles.Add(Epingle);
                _context.SaveChanges();
                return RedirectToAction("Gestion_RFQ", "AddRFQ");
            }

            return View("Create", model);
        }
    }
}