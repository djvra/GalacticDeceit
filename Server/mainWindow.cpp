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

    connect(ui->runButton, &QPushButton::clicked, this, &MainWindow::startServer);
    connect(ui->stopButton, &QPushButton::clicked, this, &MainWindow::stopServer);
    connect(ui->startGameButton, &QPushButton::clicked, server, &Server::startGame);

    for (int i = 0; i < NUM_PLAYERS; ++i) {
        // Construct the object name dynamically
        QString iconLabelName = QString("p%1iconLabel").arg(i);
        QString nameLabelName = QString("p%1nameLabel").arg(i);
        QString imposterLabelName = QString("p%1imposterLabel").arg(i);

        playerLabels[i].icon = findChild<QLabel *>(iconLabelName);
        playerLabels[i].name = findChild<QLabel *>(nameLabelName);
        playerLabels[i].imposter = findChild<QLabel *>(imposterLabelName);
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
    // Map enum values to icon file representations
    QMap<Color, QString> colorToString;
    colorToString[Red] = "red";
    colorToString[White] = "white";
    colorToString[Green] = "green";
    colorToString[Blue] = "light-blue";
    colorToString[Purple] = "violet";
    colorToString[Yellow] = "yellow";

    for (auto it = clients.begin(); it != clients.end(); ++it) {
        int id = it.key();
        ClientData data = it.value();

        QString imagePath = QString(":/assets/images/%1-among-us.png").arg(colorToString[data.getSkinColor()]);
        QPixmap image(imagePath);

        playerLabels[id].icon->setPixmap(image);
        playerLabels[id].name->setText(data.getName());
        playerLabels[id].imposter->setText(data.getImposter() ? "imposter" : "");
    }
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
            int yStart = 135;

            // Add the players
            for (auto it = clients.begin(); it != clients.end(); ++it) {                
                ClientData data = it.value();
                PlayerTransform transform = data.getPlayerTransform();
                // Create and position the player icon
                QGraphicsPixmapItem *playerIcon = new QGraphicsPixmapItem(QPixmap(":assets/images/red-among-us.png"));
                int posX = xStart + transform.getX() * 11;
                int posY = yStart + transform.getY() * -11;
                playerIcon->setPos(posX, posY);
                scene->addItem(playerIcon);
            }
        }
    }
}

