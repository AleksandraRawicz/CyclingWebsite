# Cycling Website Project

### ASP.NET Core application

## Table of contents
1. [Description](#description)
2. [Installation](#installation)
3. [Details](#details)

## Description

The Cycling Website Application is a simple project created in order to learn ASP.NET Core technology. It is a web application for cyclists that allows users to create accounts (user login and register) and publish their own trip plans. 

Each tour can contain title, summary, description, length, difficulty rating and photo-gallery. Users can also update or delete their content. Trip browsing on website is comfortable thanks to pagination system and search filters. To enlarge a feeling of efficiency application uses asynchronous operations.

Application for now is just a draft and there is no real content inside.

## Installation

In order to install and run the application:
1. Clone repository on your computer
2. Install SQL Server and optionally SQL Management Studio
3. NuGet Packages: 
All packages should be automatically restored but in case of some troubles you can find all packages used and needed in the application in CyclingWebsite.csproj file
4. In the appsettings.json file in 'EmailSettings' section set your email and password to enable sending email-confirmation link to new registered users
5. Create database using Update-Database <last migration name> command in Package Manager Console. Last migration has name: 'TourDifficultyAndTourDateAdded'

```bash

PM> Update-Database TourDifficultyAndTourDateAdded

```

## Details

Application uses:
- MVC design pattern which separates different application layers and helps to keep project organized and clean. 

- built-in IoC container

- cookie authentication
