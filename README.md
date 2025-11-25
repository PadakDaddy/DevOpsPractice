### DevOpsPractice
Software testing projects from SENG2020 (Software Quality) course - Comprehensive testing suite for TaskTracker application

### Overview
This repository demonstrates three essential testing methodologies:

-Unit Testing - Component-level testing with NUnit
-API Testing - RESTful endpoint validation
-UI Test Automation - Automated browser testing with Selenium

### Projects
## Assignment 2: Unit Testing
-9 unit tests covering 3 core functionalities

-Implements Arrange-Act-Assert (AAA) pattern

-Boundary Value Analysis for edge cases

-Tests: Task CRUD operations, search, assignee/priority management

## Assignment 3: API Testing
-18 comprehensive API tests

-Full CRUD operation coverage

-HTTP status code validation (200, 201, 204, 400, 404)

-Tests all TaskTracker RESTful endpoints

## Assignment 4: UI Test Automation
-5+ user interaction test scenarios
-Selenium WebDriver with ChromeDriver
-Real-world workflow validation
-Runs on XAMPP Apache server

## Tech Stack
-Language: C# / .NET 6.0+
-Testing Framework: NUnit
-API Testing: HttpClient
-UI Automation: Selenium WebDriver
-IDE: Visual Studio 2022
-Server: XAMPP

### Quick Start
# Unit Tests
#Open project in Visual Studio
#Navigate to Test Explorer (Test > Test Explorer)
#Run all tests or select specific tests
# API Tests
#Start TaskTracker API (default: http://localhost:5000)
#Open Test Explorer in Visual Studio
#Run API test suite
# UI Tests
#Install XAMPP and start Apache server
#Place TaskTracker in XAMPP\htdocs directory
#Ensure ChromeDriver matches your Chrome version
#Run UI tests from Test Explorer

### Project Structure
DevOpsPractice/
├── TaskTracker/              # Main application
├── TaskTracker.Tests/        # Unit tests (Assignment 2)
├── TaskTracker.API.Tests/    # API tests (Assignment 3)
├── TaskTracker.UI.Tests/     # UI automation (Assignment 4)
└── README.md                 # Documentation

### Testing Best Practices
-Arrange-Act-Assert pattern for test clarity
-Descriptive test naming conventions
-Independent test execution
-Comprehensive edge case coverage
-Proper setup/teardown lifecycle management
