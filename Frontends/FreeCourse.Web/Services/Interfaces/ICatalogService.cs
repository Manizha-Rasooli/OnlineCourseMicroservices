using FreeCourse.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<List<CourseViewModel>> GetAllCourseAsyc();
        Task<List<CategoryViewModel>> GetAllCategoryAsyc();
        Task<List<CourseViewModel>> GetAllCourseByUserIdAsyc(string userId);
        Task<CourseViewModel> GetCourseByIdAsyc(string courseId);
        Task<bool> CreateCourseAsync(CourseCreateInput courseCreateInput);
        Task<bool> UpdateCourseAsync(CourseUpdateInput courseUpdateInput);
        Task<bool>DeleteCourseAsync(string courseId);
    }
}
