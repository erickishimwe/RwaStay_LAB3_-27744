CREATE DATABASE IF NOT EXISTS rwastaydb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE rwastaydb;

CREATE TABLE IF NOT EXISTS hosts (
	id INT AUTO_INCREMENT PRIMARY KEY,
	name VARCHAR(120) NOT NULL,
	phone VARCHAR(30) NOT NULL,
	email VARCHAR(150) NOT NULL UNIQUE,
	city VARCHAR(100) NOT NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS stay_types (
	id INT AUTO_INCREMENT PRIMARY KEY,
	name VARCHAR(80) NOT NULL UNIQUE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS stays (
	id INT AUTO_INCREMENT PRIMARY KEY,
	host_id INT NOT NULL,
	title VARCHAR(150) NOT NULL,
	description TEXT,
	city VARCHAR(100) NOT NULL,
	price_per_night DECIMAL(10,2) NOT NULL,
	capacity INT NOT NULL,
	is_available BOOLEAN NOT NULL DEFAULT TRUE,
	CONSTRAINT fk_stays_host FOREIGN KEY (host_id) REFERENCES hosts(id) ON UPDATE CASCADE ON DELETE RESTRICT,
	CONSTRAINT chk_stays_price CHECK (price_per_night >= 0),
	CONSTRAINT chk_stays_capacity CHECK (capacity > 0)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS host_stay_type (
	host_id INT NOT NULL,
	stay_type_id INT NOT NULL,
	PRIMARY KEY (host_id, stay_type_id),
	CONSTRAINT fk_host_stay_type_host FOREIGN KEY (host_id) REFERENCES hosts(id) ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT fk_host_stay_type_type FOREIGN KEY (stay_type_id) REFERENCES stay_types(id) ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS guests (
	id INT AUTO_INCREMENT PRIMARY KEY,
	full_name VARCHAR(120) NOT NULL,
	email VARCHAR(150) NOT NULL UNIQUE,
	phone VARCHAR(30) NOT NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS bookings (
	id INT AUTO_INCREMENT PRIMARY KEY,
	guest_id INT NOT NULL,
	stay_id INT NOT NULL,
	check_in DATE NOT NULL,
	check_out DATE NOT NULL,
	total_price DECIMAL(10,2) NOT NULL,
	status VARCHAR(20) NOT NULL DEFAULT 'PENDING',
	CONSTRAINT fk_bookings_guest FOREIGN KEY (guest_id) REFERENCES guests(id) ON UPDATE CASCADE ON DELETE RESTRICT,
	CONSTRAINT fk_bookings_stay FOREIGN KEY (stay_id) REFERENCES stays(id) ON UPDATE CASCADE ON DELETE RESTRICT,
	CONSTRAINT chk_booking_dates CHECK (check_out > check_in),
	CONSTRAINT chk_booking_total CHECK (total_price >= 0),
	CONSTRAINT chk_booking_status CHECK (status IN ('PENDING','CONFIRMED','CANCELLED','COMPLETED'))
) ENGINE=InnoDB;

CREATE INDEX idx_stays_host_id ON stays(host_id);
CREATE INDEX idx_bookings_guest_id ON bookings(guest_id);
CREATE INDEX idx_bookings_stay_dates ON bookings(stay_id, check_in, check_out);

INSERT IGNORE INTO hosts (name, phone, email, city) VALUES
('Aline Mukamana', '+250788111222', 'aline@example.com', 'Kigali'),
('Eric Niyonzima', '+250788333444', 'eric@example.com', 'Musanze'),
('Diane Uwase', '+250788555666', 'diane@example.com', 'Rubavu');

INSERT IGNORE INTO stay_types (name) VALUES ('Hotel'), ('Guest House'), ('Apartment'), ('Villa');

INSERT IGNORE INTO stays (host_id, title, description, city, price_per_night, capacity, is_available) VALUES
(1, 'Kigali City View Hotel', 'Comfortable hotel near Kigali city center.', 'Kigali', 65000.00, 2, TRUE),
(2, 'Musanze Mountain Guest House', 'A quiet guest house with a view of the volcanoes.', 'Musanze', 45000.00, 4, TRUE),
(3, 'Lake Kivu Apartment', 'Modern apartment close to Lake Kivu.', 'Rubavu', 85000.00, 5, TRUE);

INSERT IGNORE INTO host_stay_type (host_id, stay_type_id) VALUES (1,1), (2,2), (3,3);

INSERT IGNORE INTO guests (full_name, email, phone) VALUES
('Jean Claude Habimana', 'jean@example.com', '+250788777888'),
('Claudine Ingabire', 'claudine@example.com', '+250788999000'),
('Patrick Tuyisenge', 'patrick@example.com', '+250788123456');

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
