﻿

using App.Entity;
using App.Models.Request;
using DID.Common;
using DID.Models.Base;
using Microsoft.Extensions.Logging;

namespace App.Services
{
    /// <summary>
    /// 服务接口
    /// </summary>
    public interface ICourseService
    {
        /// <summary>
        /// 获取课程信息
        /// </summary>
        /// <returns></returns>
        Task<Response<List<Course>>> GetCourse();
        /// <summary>
        /// 获取课程信息
        /// </summary>
        /// <returns></returns>
        Task<Response<Course>> GetCourse(string id);
        /// <summary>
        /// 添加课程信息
        /// </summary>
        /// <returns></returns>
        Task<Response> AddCourse(AddCourseReq req);
        /// <summary>
        /// 更新课程信息
        /// </summary>
        /// <returns></returns>
        Task<Response> UpdateCourse(Course req);
        /// <summary>
        /// 删除课程信息
        /// </summary>
        /// <returns></returns>
        Task<Response> DeleteCourse(string id);
    }

    /// <summary>
    /// 禅论系统服务
    /// </summary>
    public class CourseService : ICourseService
    {
        private readonly ILogger<CourseService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public CourseService(ILogger<CourseService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取课程信息
        /// </summary>
        /// <returns></returns>
        public async Task<Response<List<Course>>> GetCourse()
        {
            using var db = new NDatabase();
            var list = await db.FetchAsync<Course>("selece * from Course where IsDelete = 0");

            return InvokeResult.Success(list);
        }

        /// <summary>
        /// 获取课程信息
        /// </summary>
        /// <returns></returns>
        public async Task<Response<Course>> GetCourse(string id)
        {
            using var db = new NDatabase();
            var model = await db.SingleOrDefaultByIdAsync<Course>(id);

            return InvokeResult.Success(model);
        }

        /// <summary>
        /// 添加课程信息
        /// </summary>
        /// <returns></returns>
        public async Task<Response> AddCourse(AddCourseReq req)
        {
            using var db = new NDatabase();
            var model = new Course {
                CourseId = Guid.NewGuid().ToString(),
                Name = req.Name,
                Price = req.Price,
                Blurb = req.Blurb,
                Content = req.Content,
                Grade = req.Grade,
                Images = req.Images,
                TeacherId =req.TeacherId,
                CreateDate = DateTime.Now
            };
            await db.InsertAsync(model);

            return InvokeResult.Success("添加成功!");
        }

        /// <summary>
        /// 更新课程信息
        /// </summary>
        /// <returns></returns>
        public async Task<Response> UpdateCourse(Course req)
        {
            using var db = new NDatabase();
            await db.UpdateAsync(req);

            return InvokeResult.Success("更新成功!");
        }
        /// <summary>
        /// 删除课程信息
        /// </summary>
        /// <returns></returns>
        public async Task<Response> DeleteCourse(string id)
        {
            using var db = new NDatabase();
            var model = await db.SingleOrDefaultByIdAsync<Course>(id);
            model.IsDelete = DID.Entitys.IsEnum.是;
            await db.UpdateAsync(model);

            return InvokeResult.Success("删除成功!");
        }

    }
}