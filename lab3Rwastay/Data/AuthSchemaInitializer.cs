using MySqlConnector;

namespace lab3Rwastay.Data;

public static class AuthSchemaInitializer
{
    public static void Ensure()
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS users (
                id INT AUTO_INCREMENT PRIMARY KEY,
                username VARCHAR(80) NOT NULL UNIQUE,
                password_hash VARCHAR(255) NOT NULL,
                role VARCHAR(20) NOT NULL,
                host_id INT NULL,
                guest_id INT NULL,
                CONSTRAINT fk_users_host FOREIGN KEY (host_id) REFERENCES hosts(id) ON DELETE SET NULL,
                CONSTRAINT fk_users_guest FOREIGN KEY (guest_id) REFERENCES guests(id) ON DELETE SET NULL,
                CONSTRAINT chk_users_role CHECK (role IN ('ADMIN','MANAGER','CLIENT'))
            ) ENGINE=InnoDB;
            """;
        using var connection = DbConnection.Create();
        connection.Open();
        using (var command = new MySqlCommand(sql, connection)) command.ExecuteNonQuery();
        using var countCommand = new MySqlCommand("SELECT COUNT(*) FROM users WHERE role='ADMIN';", connection);
        if (Convert.ToInt32(countCommand.ExecuteScalar()) == 0)
        {
            using var seed = new MySqlCommand("INSERT INTO users (username, password_hash, role) VALUES (@username, @hash, 'ADMIN');", connection);
            seed.Parameters.AddWithValue("@username", "admin");
            seed.Parameters.AddWithValue("@hash", PasswordHasher.Hash("admin123"));
            seed.ExecuteNonQuery();
        }

        using var managerCheck = new MySqlCommand("SELECT COUNT(*) FROM users WHERE username='manager1';", connection);
        if (Convert.ToInt32(managerCheck.ExecuteScalar()) == 0)
        {
            using var manager = new MySqlCommand("INSERT INTO users (username, password_hash, role, host_id) SELECT @username, @hash, 'MANAGER', h.id FROM hosts h WHERE h.id=1;", connection);
            manager.Parameters.AddWithValue("@username", "manager1");
            manager.Parameters.AddWithValue("@hash", PasswordHasher.Hash("manager123"));
            manager.ExecuteNonQuery();
        }
    }
}
