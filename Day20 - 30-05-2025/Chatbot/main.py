import pandas as pd
import nltk
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import cosine_similarity
import string
from flask import Flask, request, jsonify

nltk.download('punkt')

class FAQChatbot:
    def __init__(self, csv_path):
        self.data = pd.read_csv(csv_path)
        self.vectorizer = TfidfVectorizer(stop_words='english')
        self.questions = self.data['Question'].apply(self._clean_text)
        self.answers = self.data['Answer']
        self.tfidf_matrix = self.vectorizer.fit_transform(self.questions)

    def _clean_text(self, text):
        return ''.join([ch.lower() for ch in text if ch not in string.punctuation])

    def get_response(self, user_input):
        user_input_clean = self._clean_text(user_input)
        user_vec = self.vectorizer.transform([user_input_clean])
        similarity = cosine_similarity(user_vec, self.tfidf_matrix)
        index = similarity.argmax()
        confidence = similarity[0][index]
        if confidence > 0.3:
            return self.answers.iloc[index]
        else:
            return "Sorry, I didn't understand that. Could you please rephrase?"


app = Flask(__name__)
bot = FAQChatbot("FAQs.csv")  # Load the chatbot with your FAQs CSV

@app.route('/chatbot', methods=['POST'])
def chatbot_response():
    data = request.get_json()
    question = data.get('question', '')
    if not question:
        return jsonify({"error": "No question provided"}), 400
    
    answer = bot.get_response(question)
    return jsonify({"answer": answer})

if __name__ == '__main__':
    app.run(debug=True)
