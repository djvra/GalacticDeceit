<<<<<<< HEAD
#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QWidget>
#include <QGraphicsView>
#include <QGraphicsScene>
#include <QGraphicsPixmapItem>
#include "server.h"
#include "player.h"

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
    void registerPlayer(PlayerInfo *player);
    void updatePlayer();

private:
    Ui::MainWindow *ui;
    Server *server;
};

#endif // MAINWINDOW_H

=======
#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QWidget>
#include <QGraphicsView>
#include <QGraphicsScene>
#include <QGraphicsPixmapItem>
#include "server.h"
#include "player.h"

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
    void registerPlayer(PlayerInfo *player);
    void updatePlayer();

private:
    Ui::MainWindow *ui;
    Server *server;
};

#endif // MAINWINDOW_H

>>>>>>> master
