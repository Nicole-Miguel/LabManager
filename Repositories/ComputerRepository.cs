using LabManager.Models;
using LabManager.Database;
using Microsoft.Data.Sqlite;
using Dapper;

namespace LabManager.Repositories;

class ComputerRepository
{
    private DatabaseConfig databaseConfig;

    public ComputerRepository(DatabaseConfig databaseConfig)
    {
        this.databaseConfig = databaseConfig;
    }
    
    public  IEnumerable<Computer> GetAll()
    {
       using var connection = new SqliteConnection(databaseConfig.ConnectionString);
       connection.Open();

       var computers = connection.Query<Computer>("SELECT * FROM Computers");

       return computers;
    }

    public Computer Save(Computer computer) 
    {
        using var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        connection.Execute("INSERT INTO Computers VALUES(@Id, @Ram, @Processor)", computer);
     
        return computer;
    }

    public Computer GetById(int id) 
    {
        var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Computers WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);

        var reader = command.ExecuteReader();
        reader.Read();
        var computer = ReaderToComputer(reader);
        
        reader.Close();
        connection.Close();

        return computer;
    }

    public Computer Update(Computer computer)
    {
        var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Computers SET ram = $ram, processor = $processor WHERE id = $id;";
        
        command.Parameters.AddWithValue("$id", computer.Id);
        command.Parameters.AddWithValue("$ram", computer.Ram);
        command.Parameters.AddWithValue("$processor", computer.Processor);

        command.ExecuteNonQuery();
        connection.Close();

        return computer;
    }

    public void Delete(int id)
    {
        using var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        connection.Execute("DELETE FROM Computers WHERE id = @Id", new {Id = id});
    }

    private Computer ReaderToComputer(SqliteDataReader reader)
    {
        var computer = new Computer(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
        return computer;
    }
  
    public bool ExistsById(int id)
    {
        var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT count(id) FROM Computers WHERE id=$id;";
        command.Parameters.AddWithValue("$id", id);

        bool result = Convert.ToBoolean(command.ExecuteScalar());

        return result; 
    }
}
