[![CircleCI](https://circleci.com/gh/RedGhoul/JobTransparency.svg?style=svg)](https://circleci.com/gh/RedGhoul/JobTransparency)
[![CodeBeat badge](https://codebeat.co/badges/cf7dd755-174f-4d09-8810-dbbb12c68c38)](https://codebeat.co/projects/github-com-redghoul-jobtransparency-master)

# Job Transparency

A Job Hunting CMS, designed to make finding a job a lot easier, by centralizing a lot of job postings, and allowing the applier to keep track of everything you want to apply to.

### Functions

- Make it easy for employers and applicants. Conveniently allows you to add jobs you find interesting into your own personal list.
- Make transparency a key feature (Lets you know how many people have been looking at a particular job, and how many people have applied to it)

### Tech Used:

#### Lanagues

- C# .Net Core 5 - ASP.NET MVC
- Python - Azure Function (To Scrap Sources)
- Python - [Text Classify](https://www.textclassify.com/)
- JavaScript - JQuery

#### Data Stores

- MySQL - Main Data Store
- Redis - For Reads of job Posts

### Hosted on Digital Ocean Using Docker & Docker Swarm
