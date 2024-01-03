#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QWidget>
#include <QGraphicsView>
#include <QGraphicsScene>
#include <QGraphicsPixmapItem>
#include "server.h"
#include "utils.h"

#define NUM_PLAYERS 6

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

private slots:
    void updateGameMap(QMap<int, ClientData> clients);
    void setPlayerLabels(QMap<int, ClientData> clients);
    void updateKilledPlayerLabel(int id);

private:
    Ui::MainWindow *ui;
    Server *server;
    PlayerLabels playerLabels[NUM_PLAYERS];
};

#endif // MAINWINDOW_H

