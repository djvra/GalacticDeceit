#include "mainWindow.h"
#include "ui_mainWindow.h"

MainWindow::MainWindow(QWidget *parent)
    : QWidget(parent)
    , ui(new Ui::MainWindow)
    , server(new Server)
{
    ui->setupUi(this);

    ui->graphicsView->setScene(new QGraphicsScene());

    ui->ipAddressLabel->setText("IP Address: " + server->getLocalIpAddress());

    connect(server, &Server::initPlayers, this, &MainWindow::setPlayerLabels);
    connect(server, &Server::updatePlayers, this, &MainWindow::updateGameMap);
    connect(server, &Server::killedPlayer, this, &MainWindow::updateKilledPlayerLabel);

    connect(ui->runButton, &QPushButton::clicked, this, &MainWindow::startServer);
    connect(ui->stopButton, &QPushButton::clicked, this, &MainWindow::stopServer);
    connect(ui->startGameButton, &QPushButton::clicked, server, &Server::startGame);

    for (int i = 0; i < NUM_PLAYERS; ++i) {
        // Construct the object name dynamically
        QString iconLabelName = QString("p%1iconLabel").arg(i);
        QString nameLabelName = QString("p%1nameLabel").arg(i);
        QString imposterLabelName = QString("p%1imposterLabel").arg(i);
        QString aliveLabelName = QString("p%1aliveLabel").arg(i);

        playerLabels[i].icon = findChild<QLabel *>(iconLabelName);
        playerLabels[i].name = findChild<QLabel *>(nameLabelName);
        playerLabels[i].imposter = findChild<QLabel *>(imposterLabelName);
        playerLabels[i].alive = findChild<QLabel *>(aliveLabelName);
    }
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::startServer()
{
    int port = ui->portLineEdit->text().toInt();
    server->start(port);
}

void MainWindow::stopServer()
{
    server->stop();
}

void MainWindow::setPlayerLabels(QMap<int, ClientData> clients)
{
    for (auto it = clients.begin(); it != clients.end(); ++it) {
        int id = it.key();
        ClientData data = it.value();

        QString imagePath = QString(":/assets/images/%1-among-us.png").arg(Constants::colorToString[data.skinColor]);
        QPixmap image(imagePath);

        playerLabels[id].icon->setPixmap(image);
        playerLabels[id].name->setText(data.name);
        playerLabels[id].imposter->setText(data.isImposter ? "imposter" : "crewmate");
        playerLabels[id].alive->setText(data.alive ? "alive" : "dead");
    }
}

void MainWindow::updateKilledPlayerLabel(int id)
{
    playerLabels[id].alive->setText("dead");
}

void MainWindow::updateGameMap(QMap<int, ClientData> clients)
{
    // Access QGraphicsView from the UI
    QGraphicsView *view = ui->graphicsView;

    if (view) {
        // Retrieve the QGraphicsScene from the QGraphicsView
        QGraphicsScene *scene = view->scene();

        if (scene) {
            // Load the game map image
            QPixmap mapImage(":/assets/images/game-map-small.png");

            // Clear the scene before adding the map image
            scene->clear();

            // Add the game map image to the scene
            scene->addPixmap(mapImage);

            int xStart = 475;
            int yStart = 130;

            // Add the players
            for (auto it = clients.begin(); it != clients.end(); ++it) {                
                ClientData data = it.value();
                PlayerTransform transform = data.playerTransform;
                // Create and position the player icon
                QString imagePath = QString(":/assets/images/%1-among-us.png").arg(Constants::colorToString[data.skinColor]);
                QGraphicsPixmapItem *playerIcon = new QGraphicsPixmapItem(QPixmap(imagePath));
                int posX = xStart + transform.x * 11;
                int posY = yStart + transform.y * -11;
                playerIcon->setPos(posX, posY);
                scene->addItem(playerIcon);
            }
        }
    }
}

