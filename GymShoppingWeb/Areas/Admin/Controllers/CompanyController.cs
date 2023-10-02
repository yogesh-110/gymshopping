using Gym.DataAccess.Repository.IRepository;
using Gym.Models;
using Gym.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymShoppingWeb.Areas.Admin.Controllers;
     [Area("Admin")]
[Authorize(Roles = "Admin")]

public class CompanyController : Controller

     { 
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }

        public IActionResult Index()
        {
            return View();
        }

        //Get

        public IActionResult Upsert(int? id)
        {
            Company company = new();         
            if (id == null || id == 0)
            {
                return View(company);
            }
            else
            {
            company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            return View(company);
            //update product
        }
            
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj, IFormFile? file)
        {


            if (ModelState.IsValid)
            {
            if (obj.Id == 0)
            {
                _unitOfWork.Company.Add(obj);
                TempData["success"] = "Company Created Successfully";
            }
            else
            {
                _unitOfWork.Company.Update(obj);
                TempData["success"] = "Company Updated Successfully";
            }
               
                _unitOfWork.Save();
              
                return RedirectToAction("Index");
            }
            return View(obj);
        }
       
        #region API CALLS  
        [HttpGet]
       public IActionResult GetAll()
       {
         var CompanyList = _unitOfWork.Company.GetAll();
         return Json(new {data= CompanyList });
       }
       //post
       [HttpDelete]

    public IActionResult Delete(int? id)
       {
        var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while Deleting" });
        }
      
        _unitOfWork.Company.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });
       
       }

      #endregion
     }
