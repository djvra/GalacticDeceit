#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QWidget>
#include <QGraphicsView>
#include <QGraphicsScene>
#include <QGraphicsPixmapItem>
#include "server.h"
#include "utils.h"

namespace Ui {
class MainWindow;
}

class MainWindow : public QWidget
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = nullptr);
    ~MainWindow();
    void startServer();
    void stopServer();
    void clearGameMap();
    void clearPlayerLabels();

private slots:
    void setGameMap(QMap<int, ClientData> clients);
    void setPlayerLabel(ClientData data);
    void setAllPlayerLabels(QMap<int, ClientData> clients);

private:
    Ui::MainWindow *ui;
    Server *server;
    PlayerLabels playerLabels[NUM_PLAYERS];
};

#endif // MAINWINDOW_H

