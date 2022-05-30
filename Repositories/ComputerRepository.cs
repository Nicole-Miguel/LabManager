using LabManager.Models;
using LabManager.Database;
using Microsoft.Data.Sqlite;

namespace LabManager.Repositories;

class ComputerRepository
{
    private DatabaseConfig databaseConfig;

    public ComputerRepository(DatabaseConfig databaseConfig)
    {
        this.databaseConfig = databaseConfig;
    }
    
    public  List<Computer> GetAll()
    {
       var computers = new List<Computer>();

       var connection = new SqliteConnection(databaseConfig.ConnectionString);
       connection.Open();

       var command = connection.CreateCommand();
       command.CommandText = "SELECT * FROM Computers;";
    
       var reader = command.ExecuteReader();
       while(reader.Read())
       {
           var computer = new Computer(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
           computers.Add(computer);
       }
       reader.Close();
       connection.Close();

       return computers;
    }

    public Computer Save(Computer computer) 
    {
        var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Computers VALUES($id, $ram, $processor);";
        command.Parameters.AddWithValue("$id", computer.Id);
        command.Parameters.AddWithValue("$ram", computer.Ram);
        command.Parameters.AddWithValue("$processor", computer.Processor);

        command.ExecuteNonQuery();
        connection.Close();

        return computer;
    }

    public Computer GetById(int id) 
    {
        var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Computers WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);

        var reader = command.ExecuteReader();
        reader.Read();
        var computer = new Computer(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
        
        reader.Close();
        connection.Close();

        return computer;
    }

    public Computer Update(Computer computer)
    {
        var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Computers SET Ram = $ram, Processor = $processor WHERE Id = $id;";
        
        command.Parameters.AddWithValue("$id", computer.Id);
        command.Parameters.AddWithValue("$ram", computer.Ram);
        command.Parameters.AddWithValue("$processor", computer.Processor);

        command.ExecuteNonQuery();
        connection.Close();

        return computer;
    }

    public void Delete(int id)
    {
        var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Computers WHERE Id = $id;";

        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
        connection.Close();
    }
}
