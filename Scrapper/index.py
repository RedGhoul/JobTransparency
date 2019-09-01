import requests
import bs4
from bs4 import BeautifulSoup
import pandas as pd
import time
from datetime import datetime

max_results_per_city = 500
postionFind = ["software+developer", "react+developer", "devops", "software+engineer"]
job_Type = "fulltime"
city_set = ["Ontario", "Vancouver"]
max_age = "15"


def Indeed():
    df_more = pd.DataFrame(
        columns=["Title", "PostDate", "Location", "Company", "Salary", "Synopsis"]
    )
    now = datetime.now()
    dt_string = now.strftime("%d/%m/%Y %H:%M:%S").replace(" ", "").replace(":", "")
    finalFileName = ("JobPostingIndeed" + dt_string).replace("/", "")
    print("saving to file: " + finalFileName)

    # scraping code:
    for city in city_set:
        for pos in postionFind:
            for start in range(0, max_results_per_city, 10):

                if start == 0:
                    print(f"Starting: City {city} & Postion Name {pos}")
                    printProgressBar(
                        0,
                        max_results_per_city,
                        prefix="Progress:",
                        suffix="Complete",
                        length=50,
                    )
                print("Request: " + str(start) + " started")
                page = requests.get(
                    "http://ca.indeed.com/jobs?q="
                    + pos
                    + "+%2420%2C000&l="
                    + str(city)
                    + "&jt="
                    + job_Type
                    + "&fromage="
                    + max_age
                    + "&start="
                    + str(start)
                )
                time.sleep(1)  # ensuring at least 1 second between page grabs
                soup = BeautifulSoup(page.text, "html.parser", from_encoding="utf-8")
                for div in soup.find_all(class_="result"):
                    for each in soup.find_all(class_="result"):
                        try:
                            title = each.find(class_="jobtitle").text.replace("\n", "")
                        except:
                            title = None
                        try:
                            location = each.find(
                                "span", {"class": "location"}
                            ).text.replace("\n", "")
                        except:
                            location = None
                        try:
                            company = each.find(class_="company").text.replace("\n", "")
                        except:
                            company = None
                        try:
                            salary = each.find(class_="salary.no-wrap").text
                        except:
                            salary = None
                        try:
                            synopsis = each.find(class_="summary").text.replace(
                                "\n", ""
                            )
                        except:
                            synopsis = None
                        try:
                            PostDate = each.find(class_="date").text.replace("\n", "")
                        except:
                            PostDate = None
                        df_more = df_more.append(
                            {
                                "Title": title,
                                "PostDate": PostDate,
                                "Location": location,
                                "Company": company,
                                "Salary": salary,
                                "Synopsis": synopsis,
                            },
                            ignore_index=True,
                        )
                    printProgressBar(
                        start + 1,
                        max_results_per_city,
                        prefix="Progress:",
                        suffix="Complete",
                        length=50,
                    )

    # saving sample_df as a local csv file — define your own local path to save contents


# Print iterations progress
def printProgressBar(
    iteration, total, prefix="", suffix="", decimals=1, length=100, fill="█"
):
    """
    Call in a loop to create terminal progress bar
    @params:
        iteration   - Required  : current iteration (Int)
        total       - Required  : total iterations (Int)
        prefix      - Optional  : prefix string (Str)
        suffix      - Optional  : suffix string (Str)
        decimals    - Optional  : positive number of decimals in percent complete (Int)
        length      - Optional  : character length of bar (Int)
        fill        - Optional  : bar fill character (Str)
    """
    percent = ("{0:." + str(decimals) + "f}").format(100 * (iteration / float(total)))
    filledLength = int(length * iteration // total)
    bar = fill * filledLength + "-" * (length - filledLength)
    print("\r%s |%s| %s%% %s" % (prefix, bar, percent, suffix), end="\r")
    # Print New Line on Complete
    if iteration == total:
        print()


if __name__ == "__main__":
    Indeed()
