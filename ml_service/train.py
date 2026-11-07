import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import TensorDataset, DataLoader
import numpy as np

from model import LSTMPricePredictor

def create_sequences(data, sequence_length):
    """
    Розбиває данні на вікна
    Наприклад, якщо sequence_length=10, то х - це 10 кроків, а у - 11-й крок.
    """

    xs, ys = [], []
    for i in range(len(data) - sequence_length):
        x = data[i:(i+sequence_length)]

        y = data[i+sequence_length][0]

        xs.append(x)
        ys.append(y)

    return np.array(xs), np.array(ys)

def train_model():
    dump_data = np.random.rand(1000, 3)

    sequence_length = 10
    x, y = create_sequences(dump_data, sequence_length=sequence_length)

    x_tensor = torch.FloatTensor(x)
    y_tensor = torch.FloatTensor(y).view(-1, 1)

    dataset = TensorDataset(x_tensor, y_tensor)
    dataloader = DataLoader(dataset, batch_size=32, shuffle=False)

    model = LSTMPricePredictor(input_size=3, hidden_size=64, num_layers=2)

    criterion = nn.MSELoss()
    optimizer = optim.Adam(model.parameters(), lr=0.001)

    epochs = 10

    print("Start of the traning!")
    model.train()

    for epoch in range(epochs):
        epoch_loss = 0
        for batch_x, batch_y in dataloader:
            optimizer.zero_grad()

            prediction = model(batch_x)

            loss = criterion(prediction, batch_y)

            loss.backward()

            optimizer.step()

            epoch_loss += loss.item()

        print(f"Epoch {epoch+1}/{epochs}. Loss {epoch_loss/len(dataloader):.4f}")

    torch.save(model.state_dict(), 'model_weights.pth')
    print("Traning saved to model_weight.pth")

if __name__ == '__main__':
    train_model()

