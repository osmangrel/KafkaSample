const express = require("express");
const app = express();
const http = require("http");
const server = http.createServer(app);

const { Server } = require("socket.io");
const io = new Server(server);

const kafka = require("kafka-node");

//Express Server
app.get("/", (req, res) => {
  res.sendFile(__dirname + "/index.html");
});

app.post("/send", (req, res) => {
  //   var channelId = req.body.correlationId;
  const message = JSON.stringify(req.body.message);
  io.emit("chat message", message);
});

//Socket.IO
io.on("connection", (socket) => {
  console.log("a user connected");

  socket.on("chat message", (msg) => {
    io.emit("chat message", msg);
    console.log("message: " + msg);
  });

  socket.on("disconnect", () => {
    console.log("user disconnected");
  });
});

server.listen(3026, () => {
  console.log("listening on *:3026");
});

// Kafka Consumer
const client = new kafka.KafkaClient({ kafkaHost: "localhost:9092" });
const consumer = new kafka.Consumer(client, [{ topic: "ms-logs" }]);
consumer.on("message", function (message) {
  console.log("Received message:", JSON.stringify(message));
  io.emit("chat message", message);
});
