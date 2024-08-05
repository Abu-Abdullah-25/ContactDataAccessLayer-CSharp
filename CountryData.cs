using Microsoft.Data.SqlClient;

namespace ContactDataAccessLayer
{
    public class CountryDTO
    {
        public int CountryID { set; get; }
        public string CountryName { set; get; }
        public string Code { set; get; }
        public string PhoneCode { set; get; }

        public CountryDTO(int countryId, string countryName, string code, string phoneCode)
        {
            CountryID = countryId;
            CountryName = countryName;
            Code = code;
            PhoneCode = phoneCode;
        }
    }

    public class CountryData
    {
        public static string _connectionString = "Server=AbuAbdullah;Database=ContactsDB;Integrated Security=True;Connection Timeout=30;TrustServerCertificate=True;";

        public static List<CountryDTO> GetAllCountries()
        {
            var countries = new List<CountryDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT CountryID,CountryName,Code,PhoneCode FROM Countries order by CountryName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int countryId = reader.GetInt32(reader.GetOrdinal("CountryID"));
                                string countryName = reader.GetString(reader.GetOrdinal("CountryName"));
                                string code = reader.GetString(reader.GetOrdinal("Code"));
                                string phoneCode = reader.GetString(reader.GetOrdinal("PhoneCode"));

                                countries.Add(new CountryDTO(countryId, countryName, code, phoneCode));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., logging or rethrowing
            }

            return countries;
        }

        public static CountryDTO GetCountryById(int countryID)
        {
            CountryDTO country = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT * FROM Countries WHERE CountryID = @CountryID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CountryID", countryID);

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                country = new CountryDTO(
                                    (int)reader["CountryID"],
                                    (string)reader["CountryName"],
                                    (string)reader["Code"],
                                    (string)reader["PhoneCode"]
                                );

                                return country;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., logging or rethrowing
            }

            return country;
        }

        public static CountryDTO GetCountryByName(string countryName)
        {
            CountryDTO country = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT * FROM Countries WHERE CountryName = @CountryName;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CountryName", countryName);

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                country = new CountryDTO(
                                    (int)reader["CountryID"],
                                    (string)reader["CountryName"],
                                    (string)reader["Code"],
                                    (string)reader["PhoneCode"]
                                );

                                return country;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., logging or rethrowing
            }

            return country;
        }

        
        public static int AddNewCountry(CountryDTO CDTO)
        {
            int countryID = -1;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"INSERT INTO Countries (CountryName,Code,PhoneCode)
                             VALUES (@CountryName,@Code,@PhoneCode);
                             SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@CountryName", CDTO.CountryName);

                        if (CDTO.Code != "")
                            command.Parameters.AddWithValue("@Code", CDTO.Code);
                        else
                            command.Parameters.AddWithValue("@Code", System.DBNull.Value);

                        if (CDTO.PhoneCode != "")
                            command.Parameters.AddWithValue("@PhoneCode", CDTO.PhoneCode);
                        else
                            command.Parameters.AddWithValue("@PhoneCode", System.DBNull.Value);

                        connection.Open();

                        // ExecuteScalar is used to retrieve the identity value of the newly added record
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            countryID = insertedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., logging or rethrowing
            }

            return countryID;
        }

        public static bool UpdateCountry(CountryDTO CDTO)
        {
            int rowsAffected = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"Update  Countries  
                            set CountryName=@CountryName,
                                Code=@Code,
                                PhoneCode=@PhoneCode
                                where CountryID = @CountryID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CountryID", CDTO.CountryID);
                        command.Parameters.AddWithValue("@CountryName", CDTO.CountryName);
                        command.Parameters.AddWithValue("@Code", CDTO.Code);
                        command.Parameters.AddWithValue("@PhoneCode", CDTO.PhoneCode);

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

        public static bool DeleteCountry(int countryID)
        {
            int rowsAffected = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"Delete Countries 
                                where CountryID = @CountryID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CountryID", countryID);

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

        public static bool IsCountryExist(int ID)
        {
            bool isFound = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT Found=1 FROM Countries WHERE CountryID = @CountryID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CountryID", ID);

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

        public static bool IsCountryExist(string CountryName)
        {
            bool isFound = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT Found=1 FROM Countries WHERE CountryName = @CountryName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CountryName", CountryName);

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


