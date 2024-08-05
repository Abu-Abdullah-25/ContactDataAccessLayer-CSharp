using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactDataAccessLayer
{
    public class ContactDTO
    {
        public int ContactID { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string Email { set; get; }
        public string Phone { set; get; }
        public string Address { set; get; }
        public DateTime DateOfBirth { set; get; }
        public int CountryID { set; get; }
        public string? ImagePath { get; set; } // Nullable string

        public ContactDTO(int ID, string FirstName, string LastName, string Email, string Phone, 
            string Address, DateTime DateOfBirth, int CountryID, string? ImagePath)

        {
            this.ContactID = ID;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.Phone = Phone;
            this.Address = Address;
            this.DateOfBirth = DateOfBirth;
            this.CountryID = CountryID;
            this.ImagePath = ImagePath;

        }
        public ContactDTO()
        {
            // Parameterless constructor required for deserialization
        }
    }

    public class ContactData
    {
        public static string _connectionString = "Server=AbuAbdullah;Database=ContactsDB;Integrated Security=True;Connection Timeout=30;TrustServerCertificate=True;";

        public static List<ContactDTO> GetAllContacts()
        {
            var contacts = new List<ContactDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT * FROM Contacts";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int contactID = reader.GetInt32(reader.GetOrdinal("ContactID"));
                                string firstName = reader.GetString(reader.GetOrdinal("FirstName"));
                                string lastName = reader.GetString(reader.GetOrdinal("LastName"));
                                string email = reader.GetString(reader.GetOrdinal("Email"));
                                string phone = reader.GetString(reader.GetOrdinal("Phone"));
                                string address = reader.GetString(reader.GetOrdinal("Address"));
                                DateTime dateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth"));
                                int countryID = reader.GetInt32(reader.GetOrdinal("CountryID"));
                                

                                string? imagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? null : reader.GetString(reader.GetOrdinal("ImagePath"));

                                contacts.Add(new ContactDTO(contactID, firstName, lastName, email, phone, address, dateOfBirth, countryID, imagePath));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., logging or rethrowing
            }

            return contacts;
        }

        public static ContactDTO GetContactById(int contactID)
        {
            ContactDTO contact = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT * FROM Contacts WHERE ContactID = @ContactID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ContactID", contactID);

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                contact = new ContactDTO(
                                    (int)reader["ContactID"],
                                    (string)reader["FirstName"],
                                    (string)reader["LastName"],
                                    (string)reader["Email"],
                                    (string)reader["Phone"],
                                    (string)reader["Address"],
                                    (DateTime)reader["DateOfBirth"],
                                    (int)reader["CountryID"],
                                    reader["ImagePath"] == DBNull.Value ? null : (string)reader["ImagePath"]
                                );

                                return contact;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., logging or rethrowing
            }

            return contact;
        }

        public static int AddNewContact(ContactDTO CDTO)
        {
            int contactID = -1;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"INSERT INTO Contacts (FirstName, LastName, Email, Phone, Address, DateOfBirth, CountryID, ImagePath)
                             VALUES (@FirstName, @LastName, @Email, @Phone, @Address, @DateOfBirth, @CountryID, @ImagePath);
                             SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", CDTO.FirstName);
                        command.Parameters.AddWithValue("@LastName", CDTO.LastName);
                        command.Parameters.AddWithValue("@Email", CDTO.Email);
                        command.Parameters.AddWithValue("@Phone", CDTO.Phone);
                        command.Parameters.AddWithValue("@Address", CDTO.Address);
                        command.Parameters.AddWithValue("@DateOfBirth", CDTO.DateOfBirth);
                        command.Parameters.AddWithValue("@CountryID", CDTO.CountryID);

                        if (CDTO.ImagePath != null)
                        {
                            command.Parameters.AddWithValue("@ImagePath", CDTO.ImagePath);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                        }

                        connection.Open();

                        // ExecuteScalar is used to retrieve the identity value of the newly added record
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            contactID = insertedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., logging or rethrowing
            }

            return contactID;
        }

        public static bool UpdateContact(ContactDTO CDTO)
        {
            int rowsAffected = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"UPDATE Contacts  
                             SET FirstName = @FirstName,
                                 LastName = @LastName,
                                 Email = @Email,
                                 Phone = @Phone,
                                 Address = @Address,
                                 DateOfBirth = @DateOfBirth,
                                 CountryID = @CountryID,
                                 ImagePath = @ImagePath
                             WHERE ContactID = @ContactID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ContactID", CDTO.ContactID);
                        command.Parameters.AddWithValue("@FirstName", CDTO.FirstName);
                        command.Parameters.AddWithValue("@LastName", CDTO.LastName);
                        command.Parameters.AddWithValue("@Email", CDTO.Email);
                        command.Parameters.AddWithValue("@Phone", CDTO.Phone);
                        command.Parameters.AddWithValue("@Address", CDTO.Address);
                        command.Parameters.AddWithValue("@DateOfBirth", CDTO.DateOfBirth);
                        command.Parameters.AddWithValue("@CountryID", CDTO.CountryID);

                        if (CDTO.ImagePath != null)
                        {
                            command.Parameters.AddWithValue("@ImagePath", CDTO.ImagePath);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                        }

                        connection.Open();

                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., logging or rethrowing
            }

            return rowsAffected > 0;
        }

        public static bool DeleteContact(int contactID)
        {
            int rowsAffected = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"DELETE FROM Contacts 
                                         WHERE ContactID = @ContactID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ContactID", contactID);

                        connection.Open();

                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., logging or rethrowing
            }

            return rowsAffected > 0;
        }

        public static bool IsContactExist(int ID)
        {
            bool isFound = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT Found=1 FROM Contacts WHERE ContactID = @ContactID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ContactID", ID);

                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        isFound = reader.HasRows;
                    }
                }
            }
            catch (Exception ex)
            {
                isFound = false;
                // Handle exceptions here, e.g., logging or rethrowing
            }

            return isFound;
        }


        

    }
}
