Car Data Scraper and User Management System

Project Overview:
A comprehensive desktop application built using WPF that not only scrapes car data from a website but also provides user management capabilities. Users can view, search, and compare various car models based on different parameters. Additionally, the system offers user authentication, account creation, and an admin control panel.

Technologies Used:

    C# (.NET Framework): Core programming language.
    WPF: Windows Presentation Foundation used for creating the GUI of the application.
    HtmlAgilityPack: Library for parsing and querying HTML content.
    OpenQA.Selenium: Automated testing framework for web applications, used here for web scraping.
    Entity Framework Core: Used for database operations and user management.
    BCrypt.Net: Library for hashing and verifying passwords.
    LiveCharts: Library for creating interactive and visually appealing charts.

Key Features:

    Concurrent Web Scraping:
        Uses Selenium with the Chrome web driver to scrape car data from a target website.
        Supports concurrent scraping of multiple pages for efficiency.

    Data Extraction, Parsing, and Comparison:
        Uses HtmlAgilityPack to parse the scraped HTML content.
        Extracts relevant car data such as brand, model, price, mileage, and more.
        Allows users to search and compare different car models using interactive charts.

    User Management:
        Provides user authentication using hashed passwords.
        Allows new users to sign up using a pin code for added security.
        Offers an admin control panel for advanced functionalities.

    Interactive Data Display:
        Displays the scraped car data in a data grid.
        Allows users to search for specific car models.
        Provides an option to compare different car models in a separate window.

    Error Handling:
        Implements robust error handling to manage potential scraping issues, validation errors, or exceptions.
        Provides user feedback via message boxes in case of errors.

    User Interface:
        Offers a user-friendly interface with buttons to initiate scraping, navigate back, logout, and more.
        Uses WPF for a responsive and modern UI design.

Challenges Overcome:

    Efficiently managing concurrent web scraping tasks without overloading the target website.
    Parsing and extracting relevant data from complex HTML structures.
    Ensuring thread safety while updating shared data structures.
    Implementing a secure user authentication and management system.

Future Enhancements:

    Implement more advanced search and filter options.
    Integrate with other data sources or APIs for a more comprehensive view.
    Enhance the comparison feature to allow side-by-side comparison of multiple car models.
    Improve user management with features like password recovery and two-factor authentication.
