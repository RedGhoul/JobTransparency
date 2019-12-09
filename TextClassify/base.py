from rake_nltk import Rake
from bs4 import BeautifulSoup
# # Extraction given the list of strings where each string is a sentence.
# r.extract_keywords_from_sentences(<list of sentences>)

# To get keyword phrases ranked highest to lowest.
# print(r.get_ranked_phrases())

# # To get keyword phrases ranked highest to lowest with scores.
# print("get_ranked_phrases_with_scores")
# print(r.get_ranked_phrases_with_scores())

import nltk

from flask import Flask,request,jsonify
app = Flask(__name__)


@app.before_first_request
def downloadNLTK():
    nltk.download('popular')

@app.route("/")
def hello():
    return "I am here to Classify"

@app.route("/Classify")
def classify():
    soup = BeautifulSoup(request.json["textIn"])
    htmlFreeText = soup.get_text()
    r = Rake()
    r.extract_keywords_from_text(htmlFreeText)
    return jsonify(r.rank_list)