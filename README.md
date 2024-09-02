# AI-Enhanced University Document Management System (AIDMS)

AIDMS API is a comprehensive backend application developed using ASP.NET Core, designed to manage academic information and documents within an educational institution efficiently. This system leverages advanced AI capabilities for document processing, automated data extraction, and intelligent validation.

## Features

- **RESTful API:** Endpoints for managing students, applications, notifications, departments, roles, documents, employees, and payments.
- **Google Cloud Integration:** Utilizes Google Cloud Vision API for document processing and Google Cloud Storage for file storage.
- **AI-Enhanced Processing:** Features Optical Character Recognition (OCR), face detection, and label detection for advanced document validation and management.
- **Security:** Robust user management and authentication using ASP.NET Core Identity and JWT for secure token-based authorization.
- **Swagger Documentation:** API endpoints are documented using Swagger for easy testing and integration.

## Technologies Used

### Programming Languages
- **C#:** Backend architecture built on C#, providing robustness, reliability, and performance optimization for handling large datasets and complex algorithms.

### Backend Framework
- **.NET Framework:** Comprehensive platform offering tools and libraries for building robust and scalable applications. Handles high traffic loads and implements built-in security features.

### Web Framework
- **ASP.NET Core:** Lightweight and modular framework for creating modern web applications, building RESTful APIs to expose functionalities to the front end.

### Database Framework
- **Entity Framework Core:** ORM framework for simplifying data access and reducing code complexity by allowing developers to work with database tables using objects.

### Authentication Framework
- **ASP.NET Core Identity:** Robust framework for user management and authentication, including features for registering, logging in, managing roles, and authorizing users.

### Authentication Token
- **JWT (JSON Web Token):** Open standard for secure communication and token-based authorization, streamlining the authentication process.

### Cloud Storage
- **Google Cloud Storage:** Secure, scalable, and cost-effective storage solution for large amounts of data, used for storing uploaded documents.

### AI Model Integration
- **Google Cloud Vision API:** Utilizes pre-trained AI models (OCR, CNN) for document validation, providing powerful capabilities for image analysis and text extraction.

### Additional Frameworks
- **MimeKit:** For sending emails and notifications.
- **UglyToad.PdfPig:** Library for reading PDF files and extracting text for validation.
- **BCrypt.Net:** Library for securely hashing passwords.

### Frontend Framework
- **HTML:** Structured web pages and organized content layout.
- **CSS:** Ensures visually appealing and consistent design across different browsers and devices.
- **JavaScript:** Adds dynamic behavior to the user interface for enhanced interactivity.
- **Angular:** Manages frontend development process, facilitating modularity, reusability, and improved code maintainability.

## Algorithms

AIDMS uses sophisticated algorithms powered by the Google Cloud Vision API to revolutionize information extraction, validation, and organization:

### Optical Character Recognition (OCR)
- **Image Preprocessing:** Enhances image quality for analysis.
- **Character Segmentation:** Isolates individual characters within the image.
- **Feature Extraction:** Analyze unique features of each segmented character.
- **Character Recognition:** Identifies characters using SVMs or deep neural networks.

### Face Detection
- **Haar Cascades:** Identifies potential face areas using rectangular filters.
- **Convolutional Neural Networks (CNNs):** Learns to identify faces by analyzing image patterns.
- **Facial Landmark Detection:** Identifies key facial features for alignment and analysis.

### Label Detection
- **Object Detection:** Uses deep learning models to detect objects within the document.
- **Image Features:** Extracts high-level features from images.
- **Classification:** Predicts the most likely label for each detected object.

## Getting Started

### Prerequisites

- [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
- SQL Server or another compatible database system
- Google Cloud Vision and Storage API credentials


## Team Members
- [Moustafa Gamal](https://github.com/MoustafaGamal01)
- [Bahaa Zenhom](https://github.com/bahaazenhom)
- [Youssef Gamal](https://github.com/YoussefGemy)
- [Mahmoud Hofny](https://github.com/mahmoudhofny)
- [Ahmed Elnersh](https://github.com/elnersh)
- [Ahmed Hofny](https://github.com/H0FNY)
- [Arwa Ashraf]()
