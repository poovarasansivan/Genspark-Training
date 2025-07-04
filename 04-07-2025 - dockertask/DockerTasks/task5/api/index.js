
// Write a docker-compose.yml file to link the API service with a MongoDB service.
// The API should connect to MongoDB and return a message from the database.

const express = require('express');
const { MongoClient } = require('mongodb');

const app = express();
const PORT = 5000;

const mongoUrl = process.env.MONGO_URL;

app.get('/api/message', async (req, res) => {
  const client = new MongoClient(mongoUrl);
  try {
    await client.connect();
    const db = client.db(); 
    const collection = db.collection('messages');
    
    if ((await collection.countDocuments()) === 0) {
      await collection.insertOne({ text: 'Hello from MongoDB!' });
    }

    const doc = await collection.findOne();
    res.json({ message: doc.text });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: 'Failed to connect to MongoDB' });
  } finally {
    await client.close();
  }
});

app.listen(PORT, () => console.log(`API listening on port ${PORT}`));
