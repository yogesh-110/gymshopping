using Gym.DataAccess.Repository.IRepository;
using Gym.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymShoppingWeb.Areas.Admin.Controllers;
     [Area("Admin")]
[Authorize(Roles = "Admin")]

public class ProductController : Controller

     { 
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _HostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment HostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _HostEnvironment = HostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Get

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,         // Set the Text property to the Name property of the retrieved object
                    Value = i.Id.ToString()  // Set the Value property to the Id property of the retrieved object converted to string
                }),

                // Initializing CoverTypeList with SelectListItem items
            };
            if (id == null || id == 0)
            {
                //create product
                //  ViewBag.CategoryList = CategoryList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                return View(productVM);
            }
            else
            {
            productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            return View(productVM);
            //update product
        }
            
        }
        //public IActionResult Delete(int? id)
        //{
        //if (id == null || id == 0)
        //{
        //    return NotFound();
        //}
        ////var categoryFromDb = _db.Categories.Find(id);
        //var ProductFromDbFirst = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
        //// var categoryFromDbSingle = _db.Categories.SingleOrDefault(u=>u.Id== id);
        //if (ProductFromDbFirst == null)
        //{
        //    return NotFound();
        //}
        //return View(ProductFromDbFirst);
        //}

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {


            if (ModelState.IsValid)
            {
                string wwwRootPath = _HostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"Images\Products");
                    var extension = Path.GetExtension(file.FileName);
                if(obj.Product.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath,obj.Product.ImageUrl.TrimStart('\\'));
                    if(System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.ImageUrl = @"\Images\Products\" + fileName + extension;
                }
            if (obj.Product.Id == 0)
            {
                _unitOfWork.Product.Add(obj.Product);
            }
            else
            {
                _unitOfWork.Product.Update(obj.Product);
            }
                //_unitOfWork.Product.Add(obj.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
       
        #region API CALLS  
        [HttpGet]
       public IActionResult GetAll()
       {
         var ProductList = _unitOfWork.Product.GetAll(includeProperties:"Category");
         return Json(new {data=ProductList});
       }
       //post
       [HttpDelete]

    public IActionResult Delete(int? id)
       {
        var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while Deleting" });
        }
        var oldImagePath = Path.Combine(_HostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }
        _unitOfWork.Product.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });
       
       }

      #endregion
     }
