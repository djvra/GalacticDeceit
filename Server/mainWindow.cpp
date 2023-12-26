#include "mainWindow.h"
#include "ui_mainWindow.h"

MainWindow::MainWindow(QWidget *parent)
    : QWidget(parent)
    , ui(new Ui::MainWindow)
    , server(new Server)
{
    ui->setupUi(this);

    ui->graphicsView->setScene(new QGraphicsScene());
    updatePlayer();

    ui->ipAddressLabel->setText("IP Address: " + server->getLocalIpAddress());

    connect(ui->runButton, &QPushButton::clicked, this, &MainWindow::startServer);
    connect(ui->stopButton, &QPushButton::clicked, this, &MainWindow::stopServer);
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
}

void MainWindow::registerPlayer(PlayerInfo *player)
{

}

void MainWindow::updatePlayer()
{

    int playerX = 300, playerY = 300;

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
            QGraphicsPixmapItem *mapPixmapItem = scene->addPixmap(mapImage);
            //view->fitInView(mapPixmapItem, Qt::KeepAspectRatio);

            // Create and position the player icon
            QGraphicsPixmapItem *playerIcon = new QGraphicsPixmapItem(QPixmap(":assets/images/red-among-us.png"));
            playerIcon->setPos(playerX, playerY);
            scene->addItem(playerIcon);
        }
    }
}
