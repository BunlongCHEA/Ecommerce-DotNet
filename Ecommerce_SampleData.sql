USE Ecommerce;


INSERT INTO [Categories] ([Name], [CreatedAt], [UpdatedAt]) VALUES
('Electronics', '2025-01-15 10:00:00', '2025-01-15 10:00:00'),
('Fashion', '2025-01-15 10:05:00', '2025-01-15 10:05:00'),
('Sports & Outdoor', '2025-01-15 10:10:00', '2025-01-15 10:10:00'),
('Home & Garden', '2025-01-15 10:15:00', '2025-01-15 10:15:00'),
('Health & Beauty', '2025-01-15 10:20:00', '2025-01-15 10:20:00'),
('Books & Media', '2025-01-15 10:25:00', '2025-01-15 10:25:00'),
('Toys & Games', '2025-01-15 10:30:00', '2025-01-15 10:30:00'),
('Automotive', '2025-01-15 10:35:00', '2025-01-15 10:35:00'),
('Food & Beverages', '2025-01-15 10:40:00', '2025-01-15 10:40:00'),
('Travel & Leisure', '2025-01-15 10:45:00', '2025-01-15 10:45:00');

INSERT INTO [SubCategories] ([Name], [CreatedAt], [UpdatedAt]) VALUES
('Laptops & Accessories', '2025-01-15 11:00:00', '2025-01-15 11:00:00'),
('Mobile Phones & Items', '2025-01-15 11:05:00', '2025-01-15 11:05:00'),
('Men Clothing', '2025-01-15 11:10:00', '2025-01-15 11:10:00'),
('Women Clothing', '2025-01-15 11:15:00', '2025-01-15 11:15:00'),
('Women Bags', '2025-01-15 11:20:00', '2025-01-15 11:20:00'),
('Sport Shoes', '2025-01-15 11:25:00', '2025-01-15 11:25:00'),
('Watches', '2025-01-15 11:30:00', '2025-01-15 11:30:00'),
('Gaming Accessories', '2025-01-15 11:35:00', '2025-01-15 11:35:00'),
('Home Appliances', '2025-01-15 11:40:00', '2025-01-15 11:40:00'),
('Sports Equipment', '2025-01-15 11:45:00', '2025-01-15 11:45:00');

INSERT INTO [LocationCountries] ([CountryName], [CountryCode], [CreatedAt], [UpdatedAt]) VALUES
('Cambodia', 'KH', '2025-01-15 12:00:00', '2025-01-15 12:00:00'),
('Thailand', 'TH', '2025-01-15 12:05:00', '2025-01-15 12:05:00'),
('Vietnam', 'VN', '2025-01-15 12:10:00', '2025-01-15 12:10:00'),
('Singapore', 'SG', '2025-01-15 12:15:00', '2025-01-15 12:15:00'),
('Malaysia', 'MY', '2025-01-15 12:20:00', '2025-01-15 12:20:00'),
('Philippines', 'PH', '2025-01-15 12:25:00', '2025-01-15 12:25:00'),
('Indonesia', 'ID', '2025-01-15 12:30:00', '2025-01-15 12:30:00'),
('Laos', 'LA', '2025-01-15 12:35:00', '2025-01-15 12:35:00'),
('Myanmar', 'MM', '2025-01-15 12:40:00', '2025-01-15 12:40:00'),
('Brunei', 'BN', '2025-01-15 12:45:00', '2025-01-15 12:45:00');

INSERT INTO [LocationRegions] ([Region], [CountryId], [CreatedAt], [UpdatedAt]) VALUES
('Phnom Penh', 1, '2025-01-15 13:00:00', '2025-01-15 13:00:00'),
('Siem Reap', 1, '2025-01-15 13:05:00', '2025-01-15 13:05:00'),
('Bangkok', 2, '2025-01-15 13:10:00', '2025-01-15 13:10:00'),
('Phuket', 2, '2025-01-15 13:15:00', '2025-01-15 13:15:00'),
('Ho Chi Minh City', 3, '2025-01-15 13:20:00', '2025-01-15 13:20:00'),
('Hanoi', 3, '2025-01-15 13:25:00', '2025-01-15 13:25:00'),
('Central Region', 4, '2025-01-15 13:30:00', '2025-01-15 13:30:00'),
('Kuala Lumpur', 5, '2025-01-15 13:35:00', '2025-01-15 13:35:00'),
('Manila', 6, '2025-01-15 13:40:00', '2025-01-15 13:40:00'),
('Jakarta', 7, '2025-01-15 13:45:00', '2025-01-15 13:45:00');

INSERT INTO [ShipmentTypes] ([Type], [CreatedAt], [UpdatedAt]) VALUES 
('Air freight', GETUTCDATE(), GETUTCDATE()),
('Ocean freight', GETUTCDATE(), GETUTCDATE()),
('Ground freight', GETUTCDATE(), GETUTCDATE());

INSERT INTO [Stores] ([Name], [Latitude], [Longitude], [CreatedAt], [UpdatedAt]) VALUES
('TechHub Cambodia', 11.5564, 104.9282, '2025-01-15 14:00:00', '2025-01-15 14:00:00'),
('Fashion Central', 11.5449, 104.8922, '2025-01-15 14:05:00', '2025-01-15 14:05:00'),
('SportZone Pro', 11.5733, 104.8927, '2025-01-15 14:10:00', '2025-01-15 14:10:00'),
('ElectroMart', 13.7563, 100.5018, '2025-01-15 14:15:00', '2025-01-15 14:15:00'),
('Style Avenue', 10.8231, 106.6297, '2025-01-15 14:20:00', '2025-01-15 14:20:00'),
('GadgetWorld', 1.3521, 103.8198, '2025-01-15 14:25:00', '2025-01-15 14:25:00'),
('Luxury Timepieces', 3.1390, 101.6869, '2025-01-15 14:30:00', '2025-01-15 14:30:00'),
('ActiveWear Store', 14.5995, 120.9842, '2025-01-15 14:35:00', '2025-01-15 14:35:00'),
('Premium Electronics', -6.2088, 106.8456, '2025-01-15 14:40:00', '2025-01-15 14:40:00'),
('Fashion Forward', 17.9757, 102.6331, '2025-01-15 14:45:00', '2025-01-15 14:45:00');

INSERT INTO [Events] ([Name], [ImageUrl], [StartDate], [EndDate], [Description], [CreatedAt], [UpdatedAt]) VALUES
('New Year Sale 2025', '', DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, 30, GETUTCDATE()), 'Biggest sale of the year with up to 70% off', '2025-01-15 15:00:00', '2025-01-15 15:00:00'),
('Tech Festival', '', DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, 30, GETUTCDATE()), 'Latest technology products showcase', '2025-01-15 15:05:00', '2025-01-15 15:05:00'),
('Spring Fashion Week', '', DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, 15, GETUTCDATE()), 'Latest spring collection launch', '2025-01-15 15:10:00', '2025-01-15 15:10:00'),
--('Sports Championship', 'https://example.com/images/sportschamp.jpg', DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, 30, GETUTCDATE()), 'Sports gear and equipment sale', '2025-01-15 15:15:00', '2025-01-15 15:15:00'),
('Summer Collection', 'https://example.com/images/summer2025.jpg', DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, 30, GETUTCDATE()), 'Summer clothing and accessories', '2025-01-15 15:20:00', '2025-01-15 15:20:00'),
--('Back to School', 'https://example.com/images/backtoschool.jpg', DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, 30, GETUTCDATE()), 'Educational and tech products for students', '2025-01-15 15:25:00', '2025-01-15 15:25:00'),
--('Mid Year Mega Sale', 'https://example.com/images/midyearsale.jpg', DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, 30, GETUTCDATE()), 'Mid year clearance sale', '2025-01-15 15:30:00', '2025-01-15 15:30:00'),
--('Luxury Watch Fair', 'https://example.com/images/watchfair.jpg', DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, 30, GETUTCDATE()), 'Premium watch collection showcase', '2025-01-15 15:35:00', '2025-01-15 15:35:00'),
--('Holiday Shopping', 'https://example.com/images/holiday2025.jpg', DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, 30, GETUTCDATE()), 'Holiday season special offers', '2025-01-15 15:40:00', '2025-01-15 15:40:00'),
('Flash Weekend Sale', 'https://example.com/images/flashsale.jpg', DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, 7, GETUTCDATE()), '48 hours flash sale event', '2025-01-15 15:45:00', '2025-01-15 15:45:00')
;

INSERT INTO [Coupons] ([Code], [Type], [DiscountPercentage], [Desription], [IsActive], [StartDate], [DurationValidity], [CreatedAt], [UpdatedAt]) VALUES
('WELCOME2025', 'User', 15.00, 'Welcome bonus for new customers', 1, DATEADD(DAY, -1, GETUTCDATE()), 365, '2025-01-15 16:00:00', '2025-01-15 16:00:00'),
('TECH20', 'Store', 20.00, '20% off on all electronics', 1, DATEADD(DAY, -1, GETUTCDATE()), 30, '2025-01-15 16:05:00', '2025-01-15 16:05:00'),
('FASHION10', 'Store', 10.00, '10% off on fashion items', 1, DATEADD(DAY, -1, GETUTCDATE()), 60, '2025-01-15 16:10:00', '2025-01-15 16:10:00'),
--('SPORT25', 'Percentage', 25.00, 'Sports equipment discount', 1, DATEADD(DAY, -1, GETUTCDATE()), 45, '2025-01-15 16:15:00', '2025-01-15 16:15:00'),
--('LUXURY5', 'Percentage', 5.00, 'Luxury items small discount', 1, DATEADD(DAY, -1, GETUTCDATE()), 90, '2025-01-15 16:20:00', '2025-01-15 16:20:00'),
--('STUDENT15', 'Percentage', 15.00, 'Student discount on electronics', 1, DATEADD(DAY, -1, GETUTCDATE()), 120, '2025-01-15 16:25:00', '2025-01-15 16:25:00'),
--('BULK30', 'Percentage', 30.00, 'Bulk purchase discount', 1, DATEADD(DAY, -1, GETUTCDATE()), 30, '2025-01-15 16:30:00', '2025-01-15 16:30:00'),
--('VIP12', 'Percentage', 12.00, 'VIP member exclusive discount', 1, DATEADD(DAY, -1, GETUTCDATE()), 180, '2025-01-15 16:35:00', '2025-01-15 16:35:00'),
--('FLASH50', 'Percentage', 50.00, 'Flash sale mega discount', 1, DATEADD(DAY, -1, GETUTCDATE()), 3, '2025-01-15 16:40:00', '2025-01-15 16:40:00'),
('LOYALTY8', 'User', 8.00, 'Loyalty program discount', 1, DATEADD(DAY, -1, GETUTCDATE()), 365, '2025-01-15 16:45:00', '2025-01-15 16:45:00');

INSERT INTO [Products] ([Name], [Price], [Description], [ImageUrl], [CategoryId], [SubCategoryId], [CouponId], [StoreId], [EventId], [CreatedAt], [UpdatedAt]) VALUES
-- Laptops & Accessories (8 products)
('MacBook Pro 16-inch M3', 2499.99, 'Latest MacBook Pro with M3 chip, 16GB RAM, 512GB SSD', 'https://example.com/macbook-pro-m3.jpg', 1, 1, 2, 1, 2, '2025-01-15 17:00:00', '2025-01-15 17:00:00'),
('Dell XPS 13 Plus', 1299.99, 'Ultra-thin laptop with Intel i7, 16GB RAM, 1TB SSD', 'https://example.com/dell-xps-13.jpg', 1, 1, 2, 1, 2, '2025-01-15 17:01:00', '2025-01-15 17:01:00'),
('HP Spectre x360', 1199.99, '2-in-1 convertible laptop with touchscreen', 'https://example.com/hp-spectre.jpg', 1, 1, 1, 4, 1, '2025-01-15 17:02:00', '2025-01-15 17:02:00'),
('ASUS ROG Gaming Laptop', 1899.99, 'High-performance gaming laptop with RTX 4070', 'https://example.com/asus-rog.jpg', 1, 1, NULL, 6, 2, '2025-01-15 17:03:00', '2025-01-15 17:03:00'),
('Laptop Cooling Pad', 49.99, 'RGB cooling pad for laptops up to 17 inches', 'https://example.com/cooling-pad.jpg', 1, 1, NULL, 1, NULL, '2025-01-15 17:04:00', '2025-01-15 17:04:00'),
('Wireless Laptop Mouse', 29.99, 'Ergonomic wireless mouse with USB-C receiver', 'https://example.com/wireless-mouse.jpg', 1, 1, NULL, 1, 1, '2025-01-15 17:05:00', '2025-01-15 17:05:00'),
('Laptop Backpack', 79.99, 'Professional laptop backpack with multiple compartments', 'https://example.com/laptop-backpack.jpg', 1, 1, NULL, 2, NULL, '2025-01-15 17:06:00', '2025-01-15 17:06:00'),
('USB-C Hub 7-in-1', 89.99, 'Multi-port USB-C hub with HDMI, USB 3.0, SD card slots', 'https://example.com/usb-hub.jpg', 1, 1, 2, 9, NULL, '2025-01-15 17:07:00', '2025-01-15 17:07:00'),

-- Mobile Phones & Items (6 products)
('iPhone 15 Pro Max', 1199.99, 'Latest iPhone with A17 Pro chip, 256GB storage', 'https://example.com/iphone-15-pro.jpg', 1, 2, 2, 6, 2, '2025-01-15 17:08:00', '2025-01-15 17:08:00'),
('Samsung Galaxy S24 Ultra', 1299.99, 'Premium Android phone with S Pen, 512GB', 'https://example.com/galaxy-s24.jpg', 1, 2, 2, 6, 2, '2025-01-15 17:09:00', '2025-01-15 17:09:00'),
('Wireless Charging Pad', 39.99, '15W fast wireless charger for all Qi devices', 'https://example.com/wireless-charger.jpg', 1, 2, NULL, 6, 1, '2025-01-15 17:10:00', '2025-01-15 17:10:00'),
('Phone Case iPhone 15', 24.99, 'Premium silicone case with screen protection', 'https://example.com/iphone-case.jpg', 1, 2, NULL, 6, NULL, '2025-01-15 17:11:00', '2025-01-15 17:11:00'),
('Bluetooth Earbuds', 149.99, 'Noise-canceling true wireless earbuds', 'https://example.com/bluetooth-earbuds.jpg', 1, 2, 1, 6, 2, '2025-01-15 17:12:00', '2025-01-15 17:12:00'),
('Phone Stand Adjustable', 19.99, 'Foldable phone stand for desk and travel', 'https://example.com/phone-stand.jpg', 1, 2, NULL, 6, NULL, '2025-01-15 17:13:00', '2025-01-15 17:13:00'),

-- Men Clothing (6 products)
('Men Casual Shirt', 49.99, '100% cotton casual shirt, multiple colors available', 'https://example.com/men-shirt.jpg', 2, 3, NULL, 2, 3, '2025-01-15 17:14:00', '2025-01-15 17:14:00'),
('Men Formal Suit', 299.99, 'Two-piece formal business suit, slim fit', 'https://example.com/men-suit.jpg', 2, 3, NULL, 5, NULL, '2025-01-15 17:15:00', '2025-01-15 17:15:00'),
('Men Jeans Premium', 79.99, 'Premium denim jeans with stretch comfort', 'https://example.com/men-jeans.jpg', 2, 3, NULL, 2, 3, '2025-01-15 17:16:00', '2025-01-15 17:16:00'),
('Men T-Shirt Pack', 39.99, '3-pack basic cotton t-shirts in assorted colors', 'https://example.com/men-tshirt.jpg', 2, 3, NULL, 2, 1, '2025-01-15 17:17:00', '2025-01-15 17:17:00'),
('Men Winter Jacket', 129.99, 'Warm winter jacket with hood, waterproof', 'https://example.com/men-jacket.jpg', 2, 3, NULL, 10, NULL, '2025-01-15 17:18:00', '2025-01-15 17:18:00'),
('Men Polo Shirt', 34.99, 'Classic polo shirt with embroidered logo', 'https://example.com/men-polo.jpg', 2, 3, NULL, 2, 3, '2025-01-15 17:19:00', '2025-01-15 17:19:00'),

-- Women Clothing (6 products)
('Women Summer Dress', 89.99, 'Elegant summer dress with floral pattern', 'https://example.com/women-dress.jpg', 2, 4, 3, 5, 5, '2025-01-15 17:20:00', '2025-01-15 17:20:00'),
('Women Blazer Professional', 159.99, 'Professional blazer for office wear', 'https://example.com/women-blazer.jpg', 2, 4, NULL, 5, NULL, '2025-01-15 17:21:00', '2025-01-15 17:21:00'),
('Women Casual Jeans', 69.99, 'High-waisted skinny jeans with stretch', 'https://example.com/women-jeans.jpg', 2, 4, NULL, 5, 3, '2025-01-15 17:22:00', '2025-01-15 17:22:00'),
('Women Blouse Silk', 79.99, '100% silk blouse with elegant design', 'https://example.com/women-blouse.jpg', 2, 4, 3, 5, 3, '2025-01-15 17:23:00', '2025-01-15 17:23:00'),
('Women Evening Gown', 249.99, 'Luxury evening gown for special occasions', 'https://example.com/women-gown.jpg', 2, 4, NULL, 5, NULL, '2025-01-15 17:24:00', '2025-01-15 17:24:00'),
('Women Cardigan', 54.99, 'Cozy knit cardigan for casual wear', 'https://example.com/women-cardigan.jpg', 2, 4, NULL, 5, NULL, '2025-01-15 17:25:00', '2025-01-15 17:25:00'),

-- Women Bags (6 products)
('Designer Handbag', 299.99, 'Luxury leather handbag with gold hardware', 'https://example.com/designer-handbag.jpg', 2, 5, NULL, 5, NULL, '2025-01-15 17:26:00', '2025-01-15 17:26:00'),
('Women Crossbody Bag', 89.99, 'Stylish crossbody bag for everyday use', 'https://example.com/crossbody-bag.jpg', 2, 5, NULL, 5, 3, '2025-01-15 17:27:00', '2025-01-15 17:27:00'),
('Women Backpack Fashion', 129.99, 'Fashion backpack with laptop compartment', 'https://example.com/women-backpack.jpg', 2, 5, NULL, 5, NULL, '2025-01-15 17:28:00', '2025-01-15 17:28:00'),
('Evening Clutch Bag', 79.99, 'Elegant clutch bag for evening events', 'https://example.com/evening-clutch.jpg', 2, 5, NULL, 5, NULL, '2025-01-15 17:29:00', '2025-01-15 17:29:00'),
('Women Tote Bag Large', 99.99, 'Large tote bag perfect for work and travel', 'https://example.com/tote-bag.jpg', 2, 5, NULL, 5, NULL, '2025-01-15 17:30:00', '2025-01-15 17:30:00'),
('Mini Shoulder Bag', 59.99, 'Cute mini shoulder bag for casual outings', 'https://example.com/mini-shoulder.jpg', 2, 5, NULL, 5, 3, '2025-01-15 17:31:00', '2025-01-15 17:31:00'),

-- Sport Shoes (6 products)
('Nike Air Max Running', 149.99, 'Premium running shoes with air cushioning', 'https://example.com/nike-airmax.jpg', 3, 6, 4, 3, 4, '2025-01-15 17:32:00', '2025-01-15 17:32:00'),
('Adidas Ultra Boost', 179.99, 'High-performance running shoes with boost technology', 'https://example.com/adidas-ultra.jpg', 3, 6, 4, 8, 4, '2025-01-15 17:33:00', '2025-01-15 17:33:00'),
('Basketball Shoes High-Top', 139.99, 'Professional basketball shoes with ankle support', 'https://example.com/basketball-shoes.jpg', 3, 6, 4, 3, 4, '2025-01-15 17:34:00', '2025-01-15 17:34:00'),
('Cross Training Shoes', 109.99, 'Versatile shoes for gym and cross training', 'https://example.com/cross-training.jpg', 3, 6, NULL, 8, 4, '2025-01-15 17:35:00', '2025-01-15 17:35:00'),
('Soccer Cleats Professional', 199.99, 'Professional soccer cleats for competitive play', 'https://example.com/soccer-cleats.jpg', 3, 6, 4, 3, 4, '2025-01-15 17:36:00', '2025-01-15 17:36:00'),
('Walking Shoes Comfort', 89.99, 'Comfortable walking shoes for daily use', 'https://example.com/walking-shoes.jpg', 3, 6, NULL, 8, NULL, '2025-01-15 17:37:00', '2025-01-15 17:37:00'),

-- Watches (6 products)
('Apple Watch Series 9', 399.99, 'Latest smartwatch with health monitoring', 'https://example.com/apple-watch.jpg', 1, 7, NULL, 7, NULL, '2025-01-15 17:38:00', '2025-01-15 17:38:00'),
('Rolex Submariner', 8999.99, 'Luxury Swiss automatic diving watch', 'https://example.com/rolex-submariner.jpg', 2, 7, NULL, 7, NULL, '2025-01-15 17:39:00', '2025-01-15 17:39:00'),
('Casio G-Shock Sports', 149.99, 'Rugged sports watch with shock resistance', 'https://example.com/gshock.jpg', 3, 7, NULL, 7, 4, '2025-01-15 17:40:00', '2025-01-15 17:40:00'),
('Seiko Automatic Classic', 299.99, 'Classic automatic watch with leather strap', 'https://example.com/seiko-automatic.jpg', 2, 7, NULL, 7, NULL, '2025-01-15 17:41:00', '2025-01-15 17:41:00'),
('Fossil Smartwatch', 199.99, 'Android Wear smartwatch with fitness tracking', 'https://example.com/fossil-smart.jpg', 1, 7, NULL, 7, 2, '2025-01-15 17:42:00', '2025-01-15 17:42:00'),
('Omega Speedmaster', 4999.99, 'Professional chronograph watch', 'https://example.com/omega-speedmaster.jpg', 2, 7, NULL, 7, NULL, '2025-01-15 17:43:00', '2025-01-15 17:43:00'),

-- Additional Mixed Products (6 products)
('Gaming Headset RGB', 79.99, 'Professional gaming headset with RGB lighting', 'https://example.com/gaming-headset.jpg', 1, 8, 2, 6, 2, '2025-01-15 17:44:00', '2025-01-15 17:44:00'),
('Coffee Machine Espresso', 299.99, 'Automatic espresso machine with milk frother', 'https://example.com/coffee-machine.jpg', 4, 9, NULL, 4, NULL, '2025-01-15 17:45:00', '2025-01-15 17:45:00'),
('Yoga Mat Premium', 49.99, 'Non-slip yoga mat with carrying strap', 'https://example.com/yoga-mat.jpg', 3, 10, 4, 8, 4, '2025-01-15 17:46:00', '2025-01-15 17:46:00'),
('Wireless Keyboard Gaming', 129.99, 'Mechanical gaming keyboard with RGB backlighting', 'https://example.com/gaming-keyboard.jpg', 1, 8, 2, 6, 2, '2025-01-15 17:47:00', '2025-01-15 17:47:00'),
('Air Purifier HEPA', 199.99, 'HEPA air purifier for rooms up to 500 sq ft', 'https://example.com/air-purifier.jpg', 4, 9, NULL, 4, NULL, '2025-01-15 17:48:00', '2025-01-15 17:48:00'),
('Fitness Tracker Band', 89.99, 'Advanced fitness tracker with heart rate monitor', 'https://example.com/fitness-tracker.jpg', 5, 7, NULL, 8, 4, '2025-01-15 17:49:00', '2025-01-15 17:49:00');
;