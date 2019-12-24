from bs4 import BeautifulSoup
from rake_nltk import Rake
import nltk
from nltk.corpus import stopwords
from nltk.cluster.util import cosine_distance
from nltk.tokenize import sent_tokenize
import numpy as np
import networkx as nx


def extractKeyPhrasesFromText(TextIn):
    soup = BeautifulSoup(TextIn)
    htmlFreeText = soup.get_text()
    htmlFreeText.replace("-","")
    htmlFreeText = htmlFreeText.strip()
    r = Rake()
    r.extract_keywords_from_text(htmlFreeText)
    final = []
    for pair in r.rank_list:
        newDic = {}
        newDic["Affinty"] = pair[0]
        newDic["Text"] = pair[1]
        final.append(newDic)
    
    return final

def read_article(Text):
    text_in = Text.split('. ')
    sentences = []
    for x in text_in:
        sentences.append(x.replace("[^a-zA-Z]", " ").split(" "))
    return sentences

def sentence_similarity(sent1, sent2, stopwords=None):
    if stopwords is None:
        stopwords = []
    sent1 = [w.lower() for w in sent1]
    sent2 = [w.lower() for w in sent2]
    all_words = list(set(sent1 + sent2))
    vector1 = [0] * len(all_words)
    vector2 = [0] * len(all_words)
    # build the vector for the first sentence
    for w in sent1:
        if w in stopwords:
            continue
        vector1[all_words.index(w)] += 1
    # build the vector for the second sentence
    for w in sent2:
        if w in stopwords:
            continue
        vector2[all_words.index(w)] += 1

    return 1 - cosine_distance(vector1, vector2)

def build_similarity_matrix(sentences, stop_words):
    # Create an empty similarity matrix
    similarity_matrix = np.zeros((len(sentences), len(sentences)))
 
    for idx1 in range(len(sentences)):
        for idx2 in range(len(sentences)):
            if idx1 == idx2: #ignore if both are same sentences
                continue 
            similarity_matrix[idx1][idx2] = sentence_similarity(sentences[idx1], sentences[idx2], stop_words)
    
    return similarity_matrix

def generate_summary(textIn, top_n=5):
    soup = BeautifulSoup(textIn)
    textIn = soup.get_text()
    textIn = textIn.replace("-","")
    textIn = textIn.strip()
    empty = []
    listtextIn = sent_tokenize(textIn)
    for x in listtextIn:
        empty.append(x)
    textIn = ". ".join(listtextIn)

    finalAnswer = []
    stop_words = stopwords.words('english')
    summarize_text = []
    # Step 1 - Read text and tokenize
    try:
        sentences =  read_article(textIn)
        # Step 2 - Generate Similary Martix across sentences
        sentence_similarity_martix = build_similarity_matrix(sentences, stop_words)
        # Step 3 - Rank sentences in similarity martix
        sentence_similarity_graph = nx.from_numpy_array(sentence_similarity_martix)
        scores = nx.pagerank(sentence_similarity_graph)
        # Step 4 - Sort the rank and pick top sentences
        ranked_sentence = sorted(((scores[i],s) for i,s in enumerate(sentences)), reverse=True)    
        #print("Indexes of top ranked_sentence order are ", ranked_sentence)

        for i in range(top_n):
            summarize_text.append(" ".join(ranked_sentence[i][1]))
            # Step 5 - Offcourse, output the summarize texr
        
        finalAnswer = ". ".join(summarize_text)

        return finalAnswer
    except:
        return ""