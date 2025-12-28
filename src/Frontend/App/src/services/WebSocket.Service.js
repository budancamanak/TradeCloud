// src/Frontend/App/src/services/WebSocket.Service.js
import * as signalR from "@microsoft/signalr";

class WebSocketService {
  constructor() {
    this.connection = null;
    this.progressHandlers = new Map();
  }

  async connect(baseUrl) {
    const token = localStorage.access_token;
    if (!token) {
      console.log("No token found, skipping WebSocket connection");
      return;
    }

    if (!baseUrl) {
      console.log("No baseUrl provided");
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${baseUrl}hubs/progress`, {
        accessTokenFactory: () => localStorage.access_token ?? "",
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.connection.on("ProgressUpdated", (executionId, progress) => {
      const handler = this.progressHandlers.get(executionId);
      if (handler?.onProgress) handler.onProgress(executionId, progress);
    });

    this.connection.on("StatusChanged", (executionId, status) => {
      console.log("StatusChanged received for executionId:", executionId, "status:", status);
      const handler = this.progressHandlers.get(executionId);
      if (handler?.onStatus) handler.onStatus(executionId, status);
    });

    this.connection.on("ExecutionCompleted", (executionId, success, error) => {
      const handler = this.progressHandlers.get(executionId);
      if (handler?.onCompleted)
        handler.onCompleted(executionId, success, error);
    });

    this.connection.onclose(() => console.log("WebSocket disconnected"));

    await this.connection.start();
    console.log("WebSocket connected");
  }

  async subscribeToExecution(executionId, handlers) {
    if (!this.connection) return;
    console.log("Subscribing to execution:", executionId);
    this.progressHandlers.set(executionId, handlers);
    await this.connection.invoke("SubscribeToExecution", executionId);
  }

  async unsubscribeFromExecution(executionId) {
    if (!this.connection) return;
    console.log("Unsubscribing from execution:", executionId);
    this.progressHandlers.delete(executionId);
    await this.connection.invoke("UnsubscribeFromExecution", executionId);
  }

  async disconnect() {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
    }
  }
}

export default new WebSocketService();
