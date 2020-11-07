![Build Status](https://dev.azure.com/Avaneesa/Job%20Transparency/_apis/build/status/RedGhoul.JobTransparency?branchName=master)

####

# Job Transparency

A Job Hunting CMS, designed to make finding a job a lot easier, by centralizing a lot of job postings, and allowing the applier to keep track of everything you want to apply to.

### Functions

- Make it easy for employers and applicants. Conveniently allows you to add jobs you find interesting into your own personal list.
- Make transparency a key feature (Lets you know how many people have been looking at a particular job, and how many people have applied to it)

### Tech Used:

#### Lanagues

- C# .Net Core 3.1 - ASP.NET MVC
- Python - Azure Function (To Scrap Indeed)
- Python - Flask Text Summarization API Server [Jobtransparency.NLP](https://github.com/RedGhoul/Jobtransparency.NLP)
- JavaScript - JQuery

#### Data Stores

- MYSQL - Main Data Store
- Redis - For Reads of job Posts
- Elastic Search - For Search (Data returned as JSON)

### Hosted on Digital Ocean Using Docker & Docker Swarm
