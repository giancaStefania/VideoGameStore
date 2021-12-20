using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading;

namespace CG.MVC5.Stefania.Models
{
    public class DBOperation
    {
        private static Semaphore semaphore = new Semaphore(0,1);
        private static SqlConnection OpenConnection()
        {
            string connection = ConfigurationManager.ConnectionStrings["DBOperationConnections"].ConnectionString;
            SqlConnection cnn = new SqlConnection(connection);
            cnn.Open();
            return cnn;
        }
        private static void CloseConnection(SqlCommand command, SqlConnection connection)
        {
            command.Dispose();
            connection.Close();
        }
        public static User CheckLogin(string email, string password)
        {
            User u = null;
            SqlConnection cnn = OpenConnection();
            string query = "account_get";
            SqlCommand command = new SqlCommand(query, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            if(email != null && password != null)
            {
                command.Parameters.AddWithValue("@email", HttpUtility.HtmlEncode(email));
                command.Parameters.AddWithValue("@password", HttpUtility.HtmlEncode(password));
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                    u = new User
                    {
                        Email = (string)dataReader.GetValue(0),
                        Password = (string)dataReader.GetValue(1),
                        Name = (string)dataReader.GetValue(2),
                        Surname = (string)dataReader.GetValue(3),   
                    };
                dataReader.Close();
            }
            CloseConnection(command, cnn);
            return u;
            
        }
        public static bool EmailExist(string email)
        {
            SqlConnection cnn = OpenConnection();
            string query = "emailExist_get";
            SqlCommand command = new SqlCommand(query, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            if (email != null)
            {
                command.Parameters.AddWithValue("@email", HttpUtility.HtmlEncode(email));
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                    if ((int)dataReader.GetValue(0) > 0)
                        return true;
                dataReader.Close();

            }
            CloseConnection(command, cnn);
            return false;
        }
        public static void SetToken(string email, string token)
        {
            DateTime date = DateTime.Now;
            DateTime expirationDate = date.AddDays(1);
            SqlConnection cnn = OpenConnection();
            string query = "token_set";
            SqlCommand command = new SqlCommand(query, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            if(email != null && token != null)
            {
                command.Parameters.AddWithValue("@email", HttpUtility.HtmlEncode(email));
                command.Parameters.AddWithValue("@token", HttpUtility.HtmlEncode(token));
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@expirationDate", expirationDate);
                command.ExecuteNonQuery();
            }
            CloseConnection(command, cnn);
        }

        public static bool CheckToken(string token)
        {
            DateTime date = DateTime.Now;
            SqlConnection cnn = OpenConnection();
            string query = "checkToken_get";
            SqlCommand command = new SqlCommand(query, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            if(token != null)
            {
                command.Parameters.AddWithValue("@token", HttpUtility.HtmlEncode(token));
                command.Parameters.AddWithValue("@date", date);
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                    if ((int)dataReader.GetValue(0) > 0)
                        return true;
                dataReader.Close();
            }
            CloseConnection(command, cnn);

            return false;

        }
        public static bool CheckEmailToken(string email)
        {
            DateTime date = DateTime.Now;
            SqlConnection cnn = OpenConnection();
            string query = "checkEmailToken_get";
            SqlCommand command = new SqlCommand(query, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            if (email != null)
            {
                command.Parameters.AddWithValue("@email", HttpUtility.HtmlEncode(email));
                command.Parameters.AddWithValue("@date", date);
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                    if ((int)dataReader.GetValue(0) > 0)
                        return true;
                dataReader.Close();
            }
            CloseConnection(command, cnn);

            return false;

        }
        public static void ModifyPassword(string password,string token)
        {
            SqlConnection cnn = OpenConnection();
            string query = "modifyPassword_set";
            SqlCommand command = new SqlCommand(query, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            if (password != null && token != null)
            {
                command.Parameters.AddWithValue("@token", HttpUtility.HtmlEncode(token));
                command.Parameters.AddWithValue("@password", HttpUtility.HtmlEncode(password));
                command.ExecuteNonQuery();
            }
            CloseConnection(command, cnn);

        }
        public static void InvalidateToken(string mail)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string query = "invalidate_token";
                SqlCommand command = new SqlCommand(query, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if(mail != null)
                {
                    command.Parameters.AddWithValue("@mail_address", HttpUtility.HtmlEncode(mail));
                    command.ExecuteNonQuery();
                }
                CloseConnection(command, cnn);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static string GetEmailToken(string token)
        {
            string email = null;
            SqlConnection cnn = OpenConnection();
            string query = "emailToken_get";
            SqlCommand command = new SqlCommand(query, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            if (token != null)
            {
                command.Parameters.AddWithValue("@token", HttpUtility.HtmlEncode(token));
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                    email = (string)dataReader.GetValue(0);
                dataReader.Close();
            }
            CloseConnection(command, cnn);

            return email;

        }
        public static string GetRole(string email)
        {
            string risultato = "";
            SqlConnection cnn = OpenConnection();
            string sql = "role_get";
            SqlCommand command = new SqlCommand(sql, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            if (email != null)
            {
                command.Parameters.AddWithValue("@email", HttpUtility.HtmlEncode(email));
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                    risultato = (string)dataReader.GetValue(0);
                dataReader.Close();
            }
            CloseConnection(command, cnn);
            return risultato;
        }
        public static List<Category> GetCategories()
        {
            List<Category> risultato = new List<Category>();
            SqlConnection cnn = OpenConnection();
            string sql = "category_get";
            SqlCommand command = new SqlCommand(sql, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                risultato.Add(
                            new Category { 
                                Id = (int)dataReader.GetValue(0), 
                                Name = (string)dataReader.GetValue(1), 
                            }
                );
            }
            dataReader.Close();
            CloseConnection(command, cnn);
            return risultato;
        }
        public static List<String> GetStock()
        {
            List<String> risultato = new List<String>();
            SqlConnection cnn = OpenConnection();
            string sql = "stock_get";
            SqlCommand command = new SqlCommand(sql, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                risultato.Add((string)dataReader.GetValue(0));
            }
            dataReader.Close();
            CloseConnection(command, cnn);
            return risultato;
        }
        public static List<String> GetPlatform()
        {
            List<String> risultato = new List<String>();
            SqlConnection cnn = OpenConnection();
            string sql = "platform_get";
            SqlCommand command = new SqlCommand(sql, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                risultato.Add((string)dataReader.GetValue(0));
            }
            dataReader.Close();
            CloseConnection(command, cnn);
            return risultato;

        }

        public static void SetProduct(ProductE p)
        {
            int id = 0 ;
            string sql = null;
            SqlConnection cnn = OpenConnection();
            sql = "product_set";
            SqlCommand command = new SqlCommand(sql, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            if (p != null)
            {
                command.Parameters.AddWithValue("@name", HttpUtility.HtmlEncode(p.Name));
                command.Parameters.AddWithValue("@description", HttpUtility.HtmlEncode(p.Description));
                command.Parameters.AddWithValue("@price", double.Parse(p.Price, CultureInfo.InvariantCulture.NumberFormat));
                command.Parameters.AddWithValue("@category", HttpUtility.HtmlEncode(p.Category));
                command.Parameters.AddWithValue("@stock", HttpUtility.HtmlEncode(p.Stock));
                command.Parameters.AddWithValue("@quantity", p.Quantity);
                if (p.Platform != null && p.Platform != "Not Selected")
                    command.Parameters.AddWithValue("@platform", HttpUtility.HtmlEncode(p.Platform));
                else
                    command.Parameters.AddWithValue("@platform", DBNull.Value);
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                    id = (int)dataReader.GetValue(0);
                dataReader.Close();
                command.Dispose();
                AddProductImage(p.Image1_file, id, cnn,true);
                AddProductImage(p.Image2_file, id, cnn,false);
                AddProductImage(p.Image3_file, id, cnn,false);
            }
            CloseConnection(command, cnn);
        }
        private static void AddProductImage(HttpPostedFileBase image,int idProduct,SqlConnection cnn, bool cover)
        {
            try
            {
                if (image != null)
                {
                    SqlCommand command2 = new SqlCommand("image_set", cnn)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure,
                    };
                    command2.Parameters.AddWithValue("@image", HttpUtility.HtmlEncode(Path.GetFileName(image.FileName)));
                    command2.Parameters.AddWithValue("@product", idProduct);
                    command2.Parameters.AddWithValue("@cover", cover);
                    command2.ExecuteNonQuery();
                    command2.Dispose();
                    FileImageMethod.SaveFile(image, idProduct);
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static List<ProductE> GetAllProduct(int page,string category,string keyword,string orderBy,string platform)
        {
            try
            {
                List<ProductE> product = new List<ProductE>();
                SqlConnection cnn = OpenConnection();
                SqlCommand command;

                string sql = "new_all_product_get";
                command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (category != null)
                    command.Parameters.AddWithValue("@name", HttpUtility.HtmlEncode(category));
                else
                    command.Parameters.AddWithValue("@name", DBNull.Value);
                if (keyword != null)
                    command.Parameters.AddWithValue("@keyword", HttpUtility.HtmlEncode(keyword));
                if (orderBy != null)
                    command.Parameters.AddWithValue("@order", HttpUtility.HtmlEncode(orderBy));
                else
                    command.Parameters.AddWithValue("@order", DBNull.Value);
                if (platform != null)
                    command.Parameters.AddWithValue("@platform", HttpUtility.HtmlEncode(platform));
                else
                    command.Parameters.AddWithValue("@platform", DBNull.Value);
                command.Parameters.AddWithValue("@page", page);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    product.Add(new ProductE
                    {
                        Id = (int)dataReader.GetValue(0),
                        Name = (string)dataReader.GetValue(1),
                        Description = (string)dataReader.GetValue(2),
                        Price = ((double)dataReader.GetValue(3)).ToString(),
                        Category = (string)dataReader.GetValue(4),
                        Stock = (string)dataReader.GetValue(5),
                        Quantity = (int)dataReader.GetValue(6),
                        Validate = (bool)dataReader.GetValue(7),
                        Platform = GetPlatformOfSingleProduct((int)dataReader.GetValue(0))
                    }
                                );
                }
                dataReader.Close();
                CloseConnection(command, cnn);

                foreach (ProductE p in product)
                {
                    List<String> image = GetProductsImages(p.Id);
                    for (int i = 0; i < image.Count(); i++)
                    {
                        if (i == 0)
                            p.Image1_string = image[i];
                        if (i == 1)
                            p.Image2_string = image[i];
                        if (i == 2)
                            p.Image3_string = image[i];
                    }


                }

                return product;
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        private static string GetPlatformOfSingleProduct(int id_product)
        {
            try
            {
                string platform = null;
                SqlConnection cnn = OpenConnection();
                string sql = "platform_of_single_product_get";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_product > 0)
                {
                    command.Parameters.AddWithValue("@id_product", id_product);
                    SqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.Read())
                        platform = (string)dataReader.GetValue(0);
                    dataReader.Close();
                    CloseConnection(command, cnn);
                }
                return platform;
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            
        }
        public static int GetNumberOfPage(string category,string keyword,string orderBy,string platform)
        {
            int result=0;
            SqlConnection cnn = OpenConnection();
            string sql = "numberPage_get";
            SqlCommand command = new SqlCommand(sql, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            if (category != null)
                command.Parameters.AddWithValue("@name", HttpUtility.HtmlEncode(category));
            else
                command.Parameters.AddWithValue("@name", DBNull.Value);
            if (keyword != null)
                command.Parameters.AddWithValue("@keyword", HttpUtility.HtmlEncode(keyword));
            
            if (orderBy != null)
                command.Parameters.AddWithValue("@order", HttpUtility.HtmlEncode(orderBy));
            else
                command.Parameters.AddWithValue("@order", DBNull.Value);
            if (platform != null)
                command.Parameters.AddWithValue("@platform", HttpUtility.HtmlEncode(platform));
            else
                command.Parameters.AddWithValue("@platform", DBNull.Value);
            SqlDataReader dataReader = command.ExecuteReader();
            if (dataReader.Read())
                result = (int)dataReader.GetValue(0);
            dataReader.Close();
            CloseConnection(command, cnn);
            return result;
        }
        private static List<String> GetProductsImages(int id)
        {
            List<String> images = new List<String>();
            SqlConnection cnn = OpenConnection();
            string sql = "images_products_get";
            SqlCommand command = new SqlCommand(sql, cnn){
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            if (id > 0)
            {
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    if (dataReader.GetValue(1) != DBNull.Value && (bool)dataReader.GetValue(1))
                        images.Insert(0, (string)dataReader.GetValue(0));
                    else
                        images.Add((string)dataReader.GetValue(0));

                }
                dataReader.Close();
                CloseConnection(command, cnn);
                return images;
            }
            return null;
        }
        public static ProductE GetSingleProduct(int id)
        {
            ProductE result = null;
            SqlConnection cnn = OpenConnection();
            string sql = "single_product_get";
            SqlCommand command = new SqlCommand(sql, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            if (id > 0)
            {
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    result = new ProductE
                    {
                        Id = (int)dataReader.GetValue(0),
                        Name = (string)dataReader.GetValue(1),
                        Description = (string)dataReader.GetValue(2),
                        Price = (string)dataReader.GetValue(3).ToString(),
                        Category = (string)dataReader.GetValue(4),
                        Stock = (string)dataReader.GetValue(5),
                        Quantity = (int)dataReader.GetValue(6),
                        Validate = (bool)dataReader.GetValue(7),
                        Platform = GetPlatformOfSingleProduct((int)dataReader.GetValue(0))
                    };
                }
                dataReader.Close();
                CloseConnection(command, cnn);
                List<String> images = GetImageSingleProduct(id);
                for (int i = 0; i < images.Count(); i++)
                {
                    if (i == 0)
                        result.Image1_string = images[i];
                    if (i == 1)
                        result.Image2_string = images[i];
                    if (i == 2)
                        result.Image3_string = images[i];
                }
            }
            
            return result;
        }
        public static List<String> GetImageSingleProduct(int id)
        {
            List<String> result = new List<String>();
            SqlConnection cnn = OpenConnection();
            string sql = "images_single_product_get";
            SqlCommand command = new SqlCommand(sql, cnn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            if(id > 0)
            {
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    if (dataReader.GetValue(1) != DBNull.Value && (bool)dataReader.GetValue(1))
                        result.Insert(0, (string)dataReader.GetValue(0));
                    else
                        result.Add((string)dataReader.GetValue(0));
                }
                dataReader.Close();
                CloseConnection(command, cnn);
            }
            return result;
        }
      
        public static void UpdateProduct(ProductE p)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "update_product_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (p != null)
                {
                    command.Parameters.AddWithValue("@id", p.Id);
                    command.Parameters.AddWithValue("@name", HttpUtility.HtmlEncode(p.Name));
                    command.Parameters.AddWithValue("@description", HttpUtility.HtmlEncode(p.Description));
                    command.Parameters.AddWithValue("@price", HttpUtility.HtmlEncode(p.Price));
                    command.Parameters.AddWithValue("@category", HttpUtility.HtmlEncode(p.Category));
                    command.Parameters.AddWithValue("@stock", HttpUtility.HtmlEncode(p.Stock));
                    command.Parameters.AddWithValue("@quantity", p.Quantity);
                    if (p.Platform != null)
                        command.Parameters.AddWithValue("@platform", HttpUtility.HtmlEncode(p.Platform));
                    else
                        command.Parameters.AddWithValue("@platform", DBNull.Value);
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);
                    UpdateImage(p,InitializeOldImageForEditProduct(p.Id));
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static void AddNewPlatform(string name_platform)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "add_platform_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (name_platform != null)
                {
                    
                    command.Parameters.AddWithValue("@name_platform", HttpUtility.HtmlEncode(name_platform));
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void DeletePlatform(string name_platform)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "delete_platform_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (name_platform != null)
                {

                    command.Parameters.AddWithValue("@name_platform", HttpUtility.HtmlEncode(name_platform));
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
        public static void DeleteProduct(int id)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string query = "delete_product_set";
                SqlCommand command = new SqlCommand(query, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id > 0)
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
                CloseConnection(command, cnn);

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
       
        private static void UpdateImage(ProductE p,List<string> oldImages)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "update_images_product_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
               
                    command.Parameters.AddWithValue("@id", p.Id);
                    
                    AddParamForUpdateImage(command, oldImages[0], p.Image1_string, 1);
                    AddParamForUpdateImage(command, oldImages[1], p.Image2_string, 2);
                    AddParamForUpdateImage(command, oldImages[2], p.Image3_string, 3);
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);
                    DeleteAllImages(p, oldImages);
                    UpdateSaveImage(oldImages[0],p.Image1_string,p.Image1_file,p.Id);
                    UpdateSaveImage(oldImages[1], p.Image2_string, p.Image2_file, p.Id);
                    UpdateSaveImage(oldImages[2], p.Image3_string, p.Image3_file, p.Id);

                

            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static List<string> InitializeOldImageForEditProduct(int id_product)
        {
            try
            {
                List<string> oldImages = DBOperation.GetImageSingleProduct(id_product);
                if (oldImages.Count() == 1)
                {
                    oldImages.Add(null);
                    oldImages.Add(null);
                }
                else
                {
                    if (oldImages.Count == 2)
                        oldImages.Add(null);
                }
                return oldImages;
            }
            catch (Exception e )
            {
                Console.WriteLine(e.Message);
                return null;
            }
            
        }
        private static void AddParamForUpdateImage(SqlCommand command,string oldImage,string newImage,int n_img)
        {
            if (oldImage != "" && oldImage != null)
                command.Parameters.AddWithValue("@old_image_" + n_img, HttpUtility.HtmlEncode(oldImage));
            else
                command.Parameters.AddWithValue("@old_image_" + n_img, DBNull.Value);
            if (newImage != "" && newImage != null)
                command.Parameters.AddWithValue("@new_image_" + n_img, HttpUtility.HtmlEncode(newImage));
            else
                command.Parameters.AddWithValue("@new_image_" + n_img, DBNull.Value);
        }
        private static void UpdateSaveImage(string oldImage, string newImage,HttpPostedFileBase file,int id) {
            if ((oldImage != newImage) && (newImage != null && newImage != "") && file != null)
                FileImageMethod.SaveFile(file, id);
        }
        private static void DeleteAllImages(ProductE p, List<string> oldImages)
        {
            if(oldImages[0] != p.Image1_string && oldImages[1] != p.Image2_string && oldImages[2] != p.Image3_string)
                FileImageMethod.DeleteAllFileDirectory(p.Id);
            else
            {
                if (oldImages[0] != p.Image1_string && oldImages[0] != null)
                    FileImageMethod.DeleteImage(oldImages[0], p.Id);
                if (oldImages[1] != p.Image2_string && oldImages[1] != null)
                    FileImageMethod.DeleteImage(oldImages[1], p.Id);
                if (oldImages[2] != p.Image3_string && oldImages[2] != null)
                    FileImageMethod.DeleteImage(oldImages[2], p.Id);
            }
        }
        public static int CheckShoppingCart(string email)
        {
            try
            {
                int result = 0; 
                SqlConnection cnn = OpenConnection();
                string sql = "check_cart_get";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (email != null)
                {
                    command.Parameters.AddWithValue("@email", HttpUtility.HtmlEncode(email));
                    SqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.Read() && dataReader.GetValue(0) != DBNull.Value)
                        result = (int)dataReader.GetValue(0);
                    dataReader.Close();
                    CloseConnection(command, cnn);
                    return result;
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public static int CreateShoppingCart(string email)
        {
            try
            {
                int result = 0;
                SqlConnection cnn = OpenConnection();
                string sql = "create_cart_get";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (email != null)
                {
                    command.Parameters.AddWithValue("@email_account", HttpUtility.HtmlEncode(email));
                    SqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.Read() && dataReader.GetValue(0) != DBNull.Value)
                        result = (int)dataReader.GetValue(0);
                    dataReader.Close();
                    CloseConnection(command, cnn);
                    return result;
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public static ShoppingCart GetShoppingCart(int id)
        {
            try
            {
                ShoppingCart result = new ShoppingCart
                {
                    Products = new List<ProductE>(),
                    Total = 0
                };
                SqlConnection cnn = OpenConnection();
                string sql = "all_products_cart_get";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id > 0)
                {
                    command.Parameters.AddWithValue("@id_cart", id);
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        result.Products.Add(new ProductE
                        {
                            Id = (int)dataReader.GetValue(0),
                            Name = (string)dataReader.GetValue(1),
                            Description = (string)dataReader.GetValue(2),
                            Price = ((double)dataReader.GetValue(3)).ToString(),
                            Category = (string)dataReader.GetValue(4),
                            Stock = (string)dataReader.GetValue(5),
                            Quantity = (int)dataReader.GetValue(6),
                            Image1_string = (string)dataReader.GetValue(7)
                        });
                        result.Total = result.Total + ((double)dataReader.GetValue(3) * (int)dataReader.GetValue(6));
                    }
                    dataReader.Close();
                    CloseConnection(command, cnn);
                    return result;
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public static void AddProductToCart(int id_cart,int id_product, int quantity)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "add_product_to_cart_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_cart > 0 && id_product > 0 && quantity > 0)
                {
                    command.Parameters.AddWithValue("@id_cart", id_cart);
                    command.Parameters.AddWithValue("@id_product", id_product);
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static int CheckIfProductIsInCart(int id_cart,int id_product)
        {
            try
            {
                int result = 0;
                SqlConnection cnn = OpenConnection();
                string sql = "check_product_is_in_cart_get";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_cart > 0 && id_product > 0)
                {
                    command.Parameters.AddWithValue("@id_cart",id_cart);
                    command.Parameters.AddWithValue("@id_product", id_product);
                    SqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.Read())
                        result = (int)dataReader.GetValue(0);
                    dataReader.Close();
                    CloseConnection(command, cnn);
                    return result;
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }

        }

        public static void IncreaseProductInCart(int id_cart, int id_product, int quantity)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "increase_quantity_product_cart_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_cart > 0 && id_product > 0 && quantity > 0)
                {
                    command.Parameters.AddWithValue("@id_cart", id_cart);
                    command.Parameters.AddWithValue("@id_product", id_product);
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void DeleteProductFromCart(int id_cart, int id_product)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "delete_element_from_cart_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_cart > 0 && id_product > 0 )
                {
                    command.Parameters.AddWithValue("@id_cart", id_cart);
                    command.Parameters.AddWithValue("@id_product", id_product);
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void IncreaseDecreaseProductFromCart(int id_cart, int id_product, int quantity)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "increase_quantity_product_cart_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_cart > 0 && id_product > 0 && quantity != 0)
                {
                    command.Parameters.AddWithValue("@id_cart", id_cart);
                    command.Parameters.AddWithValue("@id_product", id_product);
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        public static int GetQuantityOfProductCart(int id_cart, int id_product)
        {
            try
            {
                int result = 0;
                SqlConnection cnn = OpenConnection();
                string sql = "quantity_product_cart_get";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_cart > 0 && id_product > 0)
                {
                    command.Parameters.AddWithValue("@id_cart", id_cart);
                    command.Parameters.AddWithValue("@id_product", id_product);
                    SqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.Read())
                        result = (int)dataReader.GetValue(0);
                    dataReader.Close();
                    CloseConnection(command, cnn);
                    return result;
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public static int CheckOutOrder(int id_cart, string email)
        {
            try
            {
                int id_order = CreateOrder(email);
                ShoppingCart cart = GetShoppingCart(id_cart);
                bool added = AddProductsToOrder(id_order, cart);
                if (!added)
                {
                    DeleteOrder(id_order);
                    DeleteAllProductFromCart(id_cart);
                    return 0;
                }
                DeleteAllProductFromCart(id_cart);
                return id_order;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
                
            }
        }
        private static int CreateOrder(string email)
        {
            try
            {
                int result = 0;
                SqlConnection cnn = OpenConnection();
                string sql = "create_order_get";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (email != null)
                {
                    command.Parameters.AddWithValue("@date", DateTime.Now);
                    command.Parameters.AddWithValue("@email", HttpUtility.HtmlEncode(email));
                    SqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.Read())
                        result = (int)dataReader.GetValue(0);
                    dataReader.Close();
                    CloseConnection(command, cnn);
                    return result;
                }
                return result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        private static bool AddProductsToOrder(int id_order, ShoppingCart cart)
        {
            try
            {
                bool result = false;
                /*semaphore.WaitOne();*/
                    bool isAvailable = CheckProductIsAvailable(cart);
                    if (isAvailable)
                    {
                        foreach (ProductE product in cart.Products)
                        {
                            AddSingleProductToOrder(id_order, product.Id, product.Quantity);
                        }
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                /*semaphore.Release();*/
                return result;
                

            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }
        private static void AddSingleProductToOrder(int id_order,int id_product, int quantity)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "insert_product_order_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_product > 0 && quantity > 0)
                {
                    command.Parameters.AddWithValue("@id_order", id_order);
                    command.Parameters.AddWithValue("@id_product", id_product);
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static bool CheckProductIsAvailable(ShoppingCart cart)
        {
            try
            {
                bool flag = true;
                foreach(ProductE product in cart.Products)
                {
                    ProductE productAvailable = GetSingleProduct(product.Id);
                    if (productAvailable.Quantity - product.Quantity < 0)
                        flag = false;
                }
                return flag;

            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        private static void DeleteAllProductFromCart(int id_cart)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "delete_all_products_of_cart_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_cart > 0)
                {

                    command.Parameters.AddWithValue("@id_cart", id_cart);
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        private static void DeleteOrder(int id_order)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "delete_order_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_order > 0)
                {
                    command.Parameters.AddWithValue("@id_order", id_order);
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static List<OrderProducts> GetOrdersAccount(string email_account)
        {
            try
            {
                List<OrderProducts> result = new List<OrderProducts>();
                SqlConnection cnn = OpenConnection();
                string sql = "orders_account_get";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (email_account != null)
                {
                    command.Parameters.AddWithValue("@email", HttpUtility.HtmlEncode(email_account));
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read()) {
                        OrderProducts order = new OrderProducts();
                        order.Id = (int)dataReader.GetValue(0);
                        order.Date = (DateTime)dataReader.GetValue(1);
                        order.OrderState = (string)dataReader.GetValue(2);
                        order.Products = GetProductsOfOrder(order.Id);
                        result.Add(order);
                    }
                        
                    dataReader.Close();
                    CloseConnection(command, cnn);
                    return result;
                }
                return result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public static List<ProductE> GetProductsOfOrder(int id_order)
        {
            try
            {
                List<ProductE> result = new List<ProductE>();
                SqlConnection cnn = OpenConnection();
                string sql = "products_order_get";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_order > 0)
                {
                    command.Parameters.AddWithValue("@id_order", id_order);
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        ProductE product = GetSingleProduct((int)dataReader.GetValue(0));
                        product.Quantity = (int)dataReader.GetValue(1);
                        result.Add(product);
                    }

                    dataReader.Close();
                    CloseConnection(command, cnn);
                    return result;
                }
                return result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static void DeleteOrderFromUser(int id_order)
        {
            try
            {
                InvalidateOrder(id_order);
                List<ProductE> products = GetProductsOfOrder(id_order);
                foreach(ProductE product in products)
                {
                    IncreaseQuantityProduct(product.Id, product.Quantity);
                }


            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static void InvalidateOrder (int id_order)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "invalidate_order_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_order > 0)
                {
                    command.Parameters.AddWithValue("@id_order", id_order);
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static void IncreaseQuantityProduct(int id_product, int quantity)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "increase_quantity_product_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_product > 0 && quantity > 0)
                {
                    command.Parameters.AddWithValue("@id_product", id_product);
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public static string GetUserOfOrder(int id_order)
        {
            try
            {
                string result = null;
                SqlConnection cnn = OpenConnection();
                string sql = "user_of_order_get";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_order > 0)
                {
                    command.Parameters.AddWithValue("@id_order", id_order);
                    SqlDataReader dataReader = command.ExecuteReader();
                    if(dataReader.Read())
                    {
                        result = (string)dataReader.GetValue(0);
                    }

                    dataReader.Close();
                    CloseConnection(command, cnn);
                    return result;
                }
                return result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public static string GetStateOfOrder(int id_order)
        {
            try
            {
                string result = null;
                SqlConnection cnn = OpenConnection();
                string sql = "state_of_order_get";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_order > 0)
                {
                    command.Parameters.AddWithValue("@id_order", id_order);
                    SqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.Read())
                    {
                        result = (string)dataReader.GetValue(0);
                    }

                    dataReader.Close();
                    CloseConnection(command, cnn);
                    return result;
                }
                return result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public static List<int> CheckOrderContainProduct(int id_product)
        {
            try
            {
                List<int> result = new List<int>();
                SqlConnection cnn = OpenConnection();
                string sql = "check_orders_contain_product_get";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_product > 0)
                {
                    command.Parameters.AddWithValue("@id_product", id_product);
                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        result.Add((int)dataReader.GetValue(0));
                    }

                    dataReader.Close();
                    CloseConnection(command, cnn);
                    return result;
                }
                return result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public static void ChangeStateOfOrder(int id_order, string name_state)
        {
            try
            {
                SqlConnection cnn = OpenConnection();
                string sql = "change_state_of_order_set";
                SqlCommand command = new SqlCommand(sql, cnn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                };
                if (id_order > 0 && name_state != null)
                {
                    command.Parameters.AddWithValue("@id_order", id_order);
                    command.Parameters.AddWithValue("@name_state", HttpUtility.HtmlEncode(name_state));
                    command.ExecuteNonQuery();
                    CloseConnection(command, cnn);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}