#include "mainWindow.h"
#include "ui_mainWindow.h"

MainWindow::MainWindow(QWidget *parent)
    : QWidget(parent)
    , ui(new Ui::MainWindow)
    , server(new Server)
{
    this->setWindowTitle("Galactic Deceit");

    ui->setupUi(this);
    ui->graphicsView->setScene(new QGraphicsScene());
    ui->ipAddressLabel->setText("IP Address: " + server->getLocalIpAddress());

    connect(server, &Server::initPlayers, this, &MainWindow::setAllPlayerLabels);
    connect(server, &Server::updateGameMap, this, &MainWindow::setGameMap);
    connect(server, &Server::updatePlayer, this, &MainWindow::setPlayerLabel);
    connect(server, &Server::updateVotedPlayer, this, &MainWindow::setPlayerVotesLabel);
    connect(server, &Server::resetVotes, this, &MainWindow::resetPlayerVotesLabel);
    connect(server, &Server::newLogin, this, &MainWindow::setLoginLabel);

    connect(ui->startGameButton, &QPushButton::clicked, this, &MainWindow::startGame);
    connect(ui->stopGameButton, &QPushButton::clicked, this, &MainWindow::stopGame);

    for (int i = 0; i < NUM_PLAYERS; ++i) {
        // Construct the object name dynamically
        QString iconLabelName = QString("p%1iconLabel").arg(i);
        QString nameLabelName = QString("p%1nameLabel").arg(i);
        QString imposterLabelName = QString("p%1imposterLabel").arg(i);
        QString numTasksLabelName = QString("p%1numTasksLabel").arg(i);
        QString aliveLabelName = QString("p%1aliveLabel").arg(i);
        QString voteLabelName = QString("p%1voteLabel").arg(i);

        playerLabels[i].icon = findChild<QLabel *>(iconLabelName);
        playerLabels[i].name = findChild<QLabel *>(nameLabelName);
        playerLabels[i].imposter = findChild<QLabel *>(imposterLabelName);
        playerLabels[i].numTasks = findChild<QLabel *>(numTasksLabelName);
        playerLabels[i].alive = findChild<QLabel *>(aliveLabelName);
        playerLabels[i].voteReceived = findChild<QLabel *>(voteLabelName);
    }

    bool serverStatus = server->start(Constants::SERVER_TCP_PORT);
    if (serverStatus)
        ui->serverStatusLabel->setText("Server Status: Running");
    else
        ui->serverStatusLabel->setText("Server Status: Stopped");
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::startGame()
{
    server->startGame();
}

void MainWindow::stopGame()
{
    server->stopGame();
    clearGameMap();
    clearPlayerLabels();
}

void MainWindow::setLoginLabel(ClientData data)
{
    QString imagePath = QString(":/assets/images/%1-among-us.png").arg(Constants::colorToString[data.skinColor]);
    QPixmap image(imagePath);
    int id = data.id;
    playerLabels[id].icon->setPixmap(image);
    playerLabels[id].name->setText(data.name);
    playerLabels[id].imposter->setText("");
    playerLabels[id].numTasks->setText("");
    playerLabels[id].alive->setText("");
    playerLabels[id].voteReceived->setText("");
}

void MainWindow::setPlayerVotesLabel(int id, int numVotes)
{
    playerLabels[id].voteReceived->setText(QString::number(numVotes));
}

void MainWindow::resetPlayerVotesLabel()
{
    for (int i = 0; i < NUM_PLAYERS; ++i)
        playerLabels[i].voteReceived->clear();

}

void MainWindow::setPlayerLabel(ClientData data)
{
    QString imagePath = QString(":/assets/images/%1-among-us.png").arg(Constants::colorToString[data.skinColor]);
    QPixmap image(imagePath);
    int id = data.id;
    playerLabels[id].icon->setPixmap(image);
    playerLabels[id].name->setText(data.name);
    playerLabels[id].imposter->setText(data.isImposter ? "imposter" : "crewmate");
    playerLabels[id].numTasks->setText(QString::number(data.numRemainingTask));
    playerLabels[id].alive->setText(data.alive ? "alive" : "dead");
    playerLabels[id].voteReceived->setText("");
}

void MainWindow::setAllPlayerLabels(QMap<int, ClientData> clients)
{
    for (auto it = clients.begin(); it != clients.end(); ++it)
        setPlayerLabel(it.value());
}

void MainWindow::setGameMap(QMap<int, ClientData> clients)
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

                if (data.alive) {
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
}

void MainWindow::clearGameMap()
{
    // Access QGraphicsView from the UI
    QGraphicsView *view = ui->graphicsView;

    if (view) {
        // Retrieve the QGraphicsScene from the QGraphicsView
        QGraphicsScene *scene = view->scene();

        if (scene) {
            // Clear the scene before adding the map image
            scene->clear();
        }
    }
}

void MainWindow::clearPlayerLabels()
{
    for (int i = 0; i < NUM_PLAYERS; ++i) {
        playerLabels[i].icon->clear();
        playerLabels[i].name->clear();
        playerLabels[i].imposter->clear();
        playerLabels[i].numTasks->clear();
        playerLabels[i].alive->clear();
        playerLabels[i].voteReceived->clear();
    }
}

