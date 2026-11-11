USE master;
GO

-- Xóa database nếu đã tồn tại và tạo mới
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'lapdb')
BEGIN
    ALTER DATABASE lapdb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE lapdb;
END
GO

CREATE DATABASE lapdb;
GO

USE lapdb;
GO

-- 1. Bảng users
CREATE TABLE users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    full_name NVARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    phone VARCHAR(20) NOT NULL,
    password VARCHAR(255) NOT NULL,
    address NVARCHAR(MAX),
    role VARCHAR(20) NOT NULL DEFAULT 'USER'
);
GO

-- 2. Bảng categories
CREATE TABLE categories (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL UNIQUE
);
GO

INSERT INTO categories (name) VALUES 
(N'Dell'),
(N'Asus'),
(N'HP'),
(N'Lenovo'),
(N'Apple');
GO
select * from categories

-- 3. Bảng products
CREATE TABLE products (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(255) NOT NULL,
    price FLOAT NOT NULL,
    stock INT NOT NULL,
    image VARCHAR(255),
    category_id INT NOT NULL,
    FOREIGN KEY (category_id) REFERENCES categories(id)
);
GO

-- 4. Bảng product_details
CREATE TABLE product_details (
    id INT IDENTITY(1,1) PRIMARY KEY,
    cpu NVARCHAR(255),
    ram NVARCHAR(255),
    storage NVARCHAR(255),
    gpu NVARCHAR(255),
    screen NVARCHAR(255),
    battery NVARCHAR(255),
    audio NVARCHAR(255),
    ports NVARCHAR(255),
    wireless NVARCHAR(255),
    weight NVARCHAR(100),
    color NVARCHAR(100),
    os NVARCHAR(255),
    condition_status NVARCHAR(100),
    warranty NVARCHAR(100),
    more_info NVARCHAR(MAX),
    product_id INT NOT NULL UNIQUE,
    FOREIGN KEY (product_id) REFERENCES products(id)
);
GO

-- 5. Bảng cart_items
CREATE TABLE cart_items (
    id INT IDENTITY(1,1) PRIMARY KEY,
    quantity INT NOT NULL,
    user_id INT NOT NULL,
    product_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (product_id) REFERENCES products(id)
);
GO

-- 6. Bảng orders
CREATE TABLE orders (
    id INT IDENTITY(1,1) PRIMARY KEY,
    total_amount FLOAT NOT NULL,
    status NVARCHAR(50) NOT NULL,
    order_date DATETIME DEFAULT GETDATE(),
    customer_name NVARCHAR(100),
    customer_phone VARCHAR(20),
    address NVARCHAR(MAX),
    user_id INT,
    FOREIGN KEY (user_id) REFERENCES users(id)
);
GO

-- 7. Bảng order_details
CREATE TABLE order_details (
    id INT IDENTITY(1,1) PRIMARY KEY,
    quantity INT NOT NULL,
    price FLOAT NOT NULL,
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    FOREIGN KEY (order_id) REFERENCES orders(id),
    FOREIGN KEY (product_id) REFERENCES products(id)
);
GO

select * from users