/********************************************************************************
** Form generated from reading UI file 'mainWindow.ui'
**
** Created by: Qt User Interface Compiler version 6.6.1
**
** WARNING! All changes made in this file will be lost when recompiling UI file!
********************************************************************************/

#ifndef UI_MAINWINDOW_H
#define UI_MAINWINDOW_H

#include <QtCore/QVariant>
#include <QtWidgets/QApplication>
#include <QtWidgets/QGraphicsView>
#include <QtWidgets/QGridLayout>
#include <QtWidgets/QHBoxLayout>
#include <QtWidgets/QLabel>
#include <QtWidgets/QLineEdit>
#include <QtWidgets/QPushButton>
#include <QtWidgets/QSplitter>
#include <QtWidgets/QVBoxLayout>
#include <QtWidgets/QWidget>

QT_BEGIN_NAMESPACE

class Ui_MainWindow
{
public:
    QWidget *layoutWidget;
    QGridLayout *gridLayout_3;
    QLabel *p0iconLabels;
    QLabel *p0nameLabel;
    QLabel *p0imposterLabel;
    QLabel *p1iconLabel;
    QLabel *p1nameLabel;
    QLabel *p1imposterLabel;
    QLabel *p2iconLabel;
    QLabel *p2nameLabel;
    QLabel *p2imposterLabel;
    QLabel *p3iconLabel;
    QLabel *p3nameLabel;
    QLabel *p3imposterLabel;
    QLabel *p4iconLabel;
    QLabel *p4nameLabel;
    QLabel *p4imposterLabel;
    QLabel *p5iconLabel;
    QLabel *p5nameLabel;
    QLabel *p5imposterLabel;
    QGraphicsView *graphicsView;
    QWidget *layoutWidget1;
    QVBoxLayout *verticalLayout;
    QLabel *ipAddressLabel;
    QSplitter *splitter;
    QLabel *portLabel;
    QLineEdit *portLineEdit;
    QHBoxLayout *horizontalLayout;
    QPushButton *runButton;
    QPushButton *stopButton;

    void setupUi(QWidget *MainWindow)
    {
        if (MainWindow->objectName().isEmpty())
            MainWindow->setObjectName("MainWindow");
        MainWindow->resize(1049, 528);
        layoutWidget = new QWidget(MainWindow);
        layoutWidget->setObjectName("layoutWidget");
        layoutWidget->setGeometry(QRect(450, 380, 391, 140));
        gridLayout_3 = new QGridLayout(layoutWidget);
        gridLayout_3->setObjectName("gridLayout_3");
        gridLayout_3->setContentsMargins(0, 0, 0, 0);
        p0iconLabels = new QLabel(layoutWidget);
        p0iconLabels->setObjectName("p0iconLabels");
        p0iconLabels->setStyleSheet(QString::fromUtf8("image: url(:/assets/images/red-among-us.png);"));

        gridLayout_3->addWidget(p0iconLabels, 0, 0, 1, 1);

        p0nameLabel = new QLabel(layoutWidget);
        p0nameLabel->setObjectName("p0nameLabel");

        gridLayout_3->addWidget(p0nameLabel, 0, 1, 1, 1);

        p0imposterLabel = new QLabel(layoutWidget);
        p0imposterLabel->setObjectName("p0imposterLabel");

        gridLayout_3->addWidget(p0imposterLabel, 0, 2, 1, 1);

        p1iconLabel = new QLabel(layoutWidget);
        p1iconLabel->setObjectName("p1iconLabel");
        p1iconLabel->setStyleSheet(QString::fromUtf8("image: url(:/assets/images/white-among-us.png);"));

        gridLayout_3->addWidget(p1iconLabel, 1, 0, 1, 1);

        p1nameLabel = new QLabel(layoutWidget);
        p1nameLabel->setObjectName("p1nameLabel");

        gridLayout_3->addWidget(p1nameLabel, 1, 1, 1, 1);

        p1imposterLabel = new QLabel(layoutWidget);
        p1imposterLabel->setObjectName("p1imposterLabel");

        gridLayout_3->addWidget(p1imposterLabel, 1, 2, 1, 1);

        p2iconLabel = new QLabel(layoutWidget);
        p2iconLabel->setObjectName("p2iconLabel");
        p2iconLabel->setStyleSheet(QString::fromUtf8("image: url(:/assets/images/green-among-us.png);"));

        gridLayout_3->addWidget(p2iconLabel, 2, 0, 1, 1);

        p2nameLabel = new QLabel(layoutWidget);
        p2nameLabel->setObjectName("p2nameLabel");

        gridLayout_3->addWidget(p2nameLabel, 2, 1, 1, 1);

        p2imposterLabel = new QLabel(layoutWidget);
        p2imposterLabel->setObjectName("p2imposterLabel");

        gridLayout_3->addWidget(p2imposterLabel, 2, 2, 1, 1);

        p3iconLabel = new QLabel(layoutWidget);
        p3iconLabel->setObjectName("p3iconLabel");
        p3iconLabel->setStyleSheet(QString::fromUtf8("image: url(:/assets/images/light-blue-among-us.png);"));

        gridLayout_3->addWidget(p3iconLabel, 3, 0, 1, 1);

        p3nameLabel = new QLabel(layoutWidget);
        p3nameLabel->setObjectName("p3nameLabel");

        gridLayout_3->addWidget(p3nameLabel, 3, 1, 1, 1);

        p3imposterLabel = new QLabel(layoutWidget);
        p3imposterLabel->setObjectName("p3imposterLabel");

        gridLayout_3->addWidget(p3imposterLabel, 3, 2, 1, 1);

        p4iconLabel = new QLabel(layoutWidget);
        p4iconLabel->setObjectName("p4iconLabel");
        p4iconLabel->setStyleSheet(QString::fromUtf8("image: url(:/assets/images/violet-among-us.png);"));

        gridLayout_3->addWidget(p4iconLabel, 4, 0, 1, 1);

        p4nameLabel = new QLabel(layoutWidget);
        p4nameLabel->setObjectName("p4nameLabel");

        gridLayout_3->addWidget(p4nameLabel, 4, 1, 1, 1);

        p4imposterLabel = new QLabel(layoutWidget);
        p4imposterLabel->setObjectName("p4imposterLabel");

        gridLayout_3->addWidget(p4imposterLabel, 4, 2, 1, 1);

        p5iconLabel = new QLabel(layoutWidget);
        p5iconLabel->setObjectName("p5iconLabel");
        p5iconLabel->setStyleSheet(QString::fromUtf8("image: url(:/assets/images/yellow-among-us.png);"));

        gridLayout_3->addWidget(p5iconLabel, 5, 0, 1, 1);

        p5nameLabel = new QLabel(layoutWidget);
        p5nameLabel->setObjectName("p5nameLabel");

        gridLayout_3->addWidget(p5nameLabel, 5, 1, 1, 1);

        p5imposterLabel = new QLabel(layoutWidget);
        p5imposterLabel->setObjectName("p5imposterLabel");

        gridLayout_3->addWidget(p5imposterLabel, 5, 2, 1, 1);

        graphicsView = new QGraphicsView(MainWindow);
        graphicsView->setObjectName("graphicsView");
        graphicsView->setGeometry(QRect(220, 10, 801, 341));
        graphicsView->setStyleSheet(QString::fromUtf8("background-color: rgb(0, 0, 0);"));
        layoutWidget1 = new QWidget(MainWindow);
        layoutWidget1->setObjectName("layoutWidget1");
        layoutWidget1->setGeometry(QRect(20, 70, 170, 86));
        verticalLayout = new QVBoxLayout(layoutWidget1);
        verticalLayout->setObjectName("verticalLayout");
        verticalLayout->setContentsMargins(0, 0, 0, 0);
        ipAddressLabel = new QLabel(layoutWidget1);
        ipAddressLabel->setObjectName("ipAddressLabel");

        verticalLayout->addWidget(ipAddressLabel);

        splitter = new QSplitter(layoutWidget1);
        splitter->setObjectName("splitter");
        splitter->setOrientation(Qt::Horizontal);
        portLabel = new QLabel(splitter);
        portLabel->setObjectName("portLabel");
        splitter->addWidget(portLabel);
        portLineEdit = new QLineEdit(splitter);
        portLineEdit->setObjectName("portLineEdit");
        splitter->addWidget(portLineEdit);

        verticalLayout->addWidget(splitter);

        horizontalLayout = new QHBoxLayout();
        horizontalLayout->setObjectName("horizontalLayout");
        runButton = new QPushButton(layoutWidget1);
        runButton->setObjectName("runButton");
        runButton->setAutoDefault(false);

        horizontalLayout->addWidget(runButton);

        stopButton = new QPushButton(layoutWidget1);
        stopButton->setObjectName("stopButton");

        horizontalLayout->addWidget(stopButton);


        verticalLayout->addLayout(horizontalLayout);


        retranslateUi(MainWindow);

        QMetaObject::connectSlotsByName(MainWindow);
    } // setupUi

    void retranslateUi(QWidget *MainWindow)
    {
        MainWindow->setWindowTitle(QCoreApplication::translate("MainWindow", "Form", nullptr));
        p0iconLabels->setText(QString());
        p0nameLabel->setText(QCoreApplication::translate("MainWindow", "Player", nullptr));
        p0imposterLabel->setText(QCoreApplication::translate("MainWindow", "info", nullptr));
        p1iconLabel->setText(QString());
        p1nameLabel->setText(QCoreApplication::translate("MainWindow", "Player", nullptr));
        p1imposterLabel->setText(QCoreApplication::translate("MainWindow", "info", nullptr));
        p2iconLabel->setText(QString());
        p2nameLabel->setText(QCoreApplication::translate("MainWindow", "Player", nullptr));
        p2imposterLabel->setText(QCoreApplication::translate("MainWindow", "info", nullptr));
        p3iconLabel->setText(QString());
        p3nameLabel->setText(QCoreApplication::translate("MainWindow", "Player", nullptr));
        p3imposterLabel->setText(QCoreApplication::translate("MainWindow", "info", nullptr));
        p4iconLabel->setText(QString());
        p4nameLabel->setText(QCoreApplication::translate("MainWindow", "Player", nullptr));
        p4imposterLabel->setText(QCoreApplication::translate("MainWindow", "info", nullptr));
        p5iconLabel->setText(QString());
        p5nameLabel->setText(QCoreApplication::translate("MainWindow", "Player", nullptr));
        p5imposterLabel->setText(QCoreApplication::translate("MainWindow", "info", nullptr));
        ipAddressLabel->setText(QCoreApplication::translate("MainWindow", "Server IP: ", nullptr));
        portLabel->setText(QCoreApplication::translate("MainWindow", "Port:", nullptr));
        portLineEdit->setText(QCoreApplication::translate("MainWindow", "9000", nullptr));
        runButton->setText(QCoreApplication::translate("MainWindow", "Run", nullptr));
        stopButton->setText(QCoreApplication::translate("MainWindow", "Stop", nullptr));
    } // retranslateUi

};

namespace Ui {
    class MainWindow: public Ui_MainWindow {};
} // namespace Ui

QT_END_NAMESPACE

#endif // UI_MAINWINDOW_H
