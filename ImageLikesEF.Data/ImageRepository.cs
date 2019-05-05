using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ImageLikesEF.Data
{
    public class ImageRepository
    {
        private string _connectionString;

        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Image> GetImages()
        {
            using (var context = new ImageContext(_connectionString))
            {
                return context.Images.OrderByDescending(i => i.Date).ToList();
            }
        }

        public void AddImage(Image image)
        {
            var context = new ImageContext(_connectionString);
            context.Images.Add(image);
            context.SaveChanges();
        }

        public Image GetImage(int id)
        {
            using (var context = new ImageContext(_connectionString))
            {
                return context.Images.FirstOrDefault(i => i.Id == id);
            }
        }

        public void SetLike(int id)
        {
            using (var context = new ImageContext(_connectionString))
            {
                context.Database.ExecuteSqlCommand(
                    @"UPDATE Images
                    SET Likes = Likes + 1
                    WHERE id = @id",
                    new SqlParameter("@id", id));
                }
        }

        public int GetLikes(int id)
        {
            using (var context = new ImageContext(_connectionString))
            {
                return context.Images.FirstOrDefault(i => i.Id == id).Likes;
            }
        }
    }
}
