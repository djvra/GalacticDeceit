/****************************************************************************
** Meta object code from reading C++ file 'server.h'
**
** Created by: The Qt Meta Object Compiler version 68 (Qt 6.6.1)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include "../../Server/server.h"
#include <QtGui/qtextcursor.h>
#include <QtCore/qmetatype.h>

#if __has_include(<QtCore/qtmochelpers.h>)
#include <QtCore/qtmochelpers.h>
#else
QT_BEGIN_MOC_NAMESPACE
#endif


#include <memory>

#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'server.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 68
#error "This file was generated using the moc from 6.6.1. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

#ifndef Q_CONSTINIT
#define Q_CONSTINIT
#endif

QT_WARNING_PUSH
QT_WARNING_DISABLE_DEPRECATED
QT_WARNING_DISABLE_GCC("-Wuseless-cast")
namespace {

#ifdef QT_MOC_HAS_STRINGDATA
struct qt_meta_stringdata_CLASSServerENDCLASS_t {};
static constexpr auto qt_meta_stringdata_CLASSServerENDCLASS = QtMocHelpers::stringData(
    "Server",
    "initPlayers",
    "",
    "QMap<int,ClientData>",
    "clients",
    "updatePlayers",
    "handleNewTcpConnection",
    "handleTcpData",
    "QTcpSocket*",
    "socket",
    "handleUdpDatagrams",
    "sendPlayerId",
    "sendPlayerData"
);
#else  // !QT_MOC_HAS_STRING_DATA
struct qt_meta_stringdata_CLASSServerENDCLASS_t {
    uint offsetsAndSizes[26];
    char stringdata0[7];
    char stringdata1[12];
    char stringdata2[1];
    char stringdata3[21];
    char stringdata4[8];
    char stringdata5[14];
    char stringdata6[23];
    char stringdata7[14];
    char stringdata8[12];
    char stringdata9[7];
    char stringdata10[19];
    char stringdata11[13];
    char stringdata12[15];
};
#define QT_MOC_LITERAL(ofs, len) \
    uint(sizeof(qt_meta_stringdata_CLASSServerENDCLASS_t::offsetsAndSizes) + ofs), len 
Q_CONSTINIT static const qt_meta_stringdata_CLASSServerENDCLASS_t qt_meta_stringdata_CLASSServerENDCLASS = {
    {
        QT_MOC_LITERAL(0, 6),  // "Server"
        QT_MOC_LITERAL(7, 11),  // "initPlayers"
        QT_MOC_LITERAL(19, 0),  // ""
        QT_MOC_LITERAL(20, 20),  // "QMap<int,ClientData>"
        QT_MOC_LITERAL(41, 7),  // "clients"
        QT_MOC_LITERAL(49, 13),  // "updatePlayers"
        QT_MOC_LITERAL(63, 22),  // "handleNewTcpConnection"
        QT_MOC_LITERAL(86, 13),  // "handleTcpData"
        QT_MOC_LITERAL(100, 11),  // "QTcpSocket*"
        QT_MOC_LITERAL(112, 6),  // "socket"
        QT_MOC_LITERAL(119, 18),  // "handleUdpDatagrams"
        QT_MOC_LITERAL(138, 12),  // "sendPlayerId"
        QT_MOC_LITERAL(151, 14)   // "sendPlayerData"
    },
    "Server",
    "initPlayers",
    "",
    "QMap<int,ClientData>",
    "clients",
    "updatePlayers",
    "handleNewTcpConnection",
    "handleTcpData",
    "QTcpSocket*",
    "socket",
    "handleUdpDatagrams",
    "sendPlayerId",
    "sendPlayerData"
};
#undef QT_MOC_LITERAL
#endif // !QT_MOC_HAS_STRING_DATA
} // unnamed namespace

Q_CONSTINIT static const uint qt_meta_data_CLASSServerENDCLASS[] = {

 // content:
      12,       // revision
       0,       // classname
       0,    0, // classinfo
       7,   14, // methods
       0,    0, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       2,       // signalCount

 // signals: name, argc, parameters, tag, flags, initial metatype offsets
       1,    1,   56,    2, 0x06,    1 /* Public */,
       5,    1,   59,    2, 0x06,    3 /* Public */,

 // slots: name, argc, parameters, tag, flags, initial metatype offsets
       6,    0,   62,    2, 0x08,    5 /* Private */,
       7,    1,   63,    2, 0x08,    6 /* Private */,
      10,    0,   66,    2, 0x08,    8 /* Private */,
      11,    0,   67,    2, 0x08,    9 /* Private */,
      12,    0,   68,    2, 0x08,   10 /* Private */,

 // signals: parameters
    QMetaType::Void, 0x80000000 | 3,    4,
    QMetaType::Void, 0x80000000 | 3,    4,

 // slots: parameters
    QMetaType::Void,
    QMetaType::Void, 0x80000000 | 8,    9,
    QMetaType::Void,
    QMetaType::Void,
    QMetaType::Void,

       0        // eod
};

Q_CONSTINIT const QMetaObject Server::staticMetaObject = { {
    QMetaObject::SuperData::link<QWidget::staticMetaObject>(),
    qt_meta_stringdata_CLASSServerENDCLASS.offsetsAndSizes,
    qt_meta_data_CLASSServerENDCLASS,
    qt_static_metacall,
    nullptr,
    qt_incomplete_metaTypeArray<qt_meta_stringdata_CLASSServerENDCLASS_t,
        // Q_OBJECT / Q_GADGET
        QtPrivate::TypeAndForceComplete<Server, std::true_type>,
        // method 'initPlayers'
        QtPrivate::TypeAndForceComplete<void, std::false_type>,
        QtPrivate::TypeAndForceComplete<QMap<int,ClientData>, std::false_type>,
        // method 'updatePlayers'
        QtPrivate::TypeAndForceComplete<void, std::false_type>,
        QtPrivate::TypeAndForceComplete<QMap<int,ClientData>, std::false_type>,
        // method 'handleNewTcpConnection'
        QtPrivate::TypeAndForceComplete<void, std::false_type>,
        // method 'handleTcpData'
        QtPrivate::TypeAndForceComplete<void, std::false_type>,
        QtPrivate::TypeAndForceComplete<QTcpSocket *, std::false_type>,
        // method 'handleUdpDatagrams'
        QtPrivate::TypeAndForceComplete<void, std::false_type>,
        // method 'sendPlayerId'
        QtPrivate::TypeAndForceComplete<void, std::false_type>,
        // method 'sendPlayerData'
        QtPrivate::TypeAndForceComplete<void, std::false_type>
    >,
    nullptr
} };

void Server::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        auto *_t = static_cast<Server *>(_o);
        (void)_t;
        switch (_id) {
        case 0: _t->initPlayers((*reinterpret_cast< std::add_pointer_t<QMap<int,ClientData>>>(_a[1]))); break;
        case 1: _t->updatePlayers((*reinterpret_cast< std::add_pointer_t<QMap<int,ClientData>>>(_a[1]))); break;
        case 2: _t->handleNewTcpConnection(); break;
        case 3: _t->handleTcpData((*reinterpret_cast< std::add_pointer_t<QTcpSocket*>>(_a[1]))); break;
        case 4: _t->handleUdpDatagrams(); break;
        case 5: _t->sendPlayerId(); break;
        case 6: _t->sendPlayerData(); break;
        default: ;
        }
    } else if (_c == QMetaObject::RegisterMethodArgumentMetaType) {
        switch (_id) {
        default: *reinterpret_cast<QMetaType *>(_a[0]) = QMetaType(); break;
        case 3:
            switch (*reinterpret_cast<int*>(_a[1])) {
            default: *reinterpret_cast<QMetaType *>(_a[0]) = QMetaType(); break;
            case 0:
                *reinterpret_cast<QMetaType *>(_a[0]) = QMetaType::fromType< QTcpSocket* >(); break;
            }
            break;
        }
    } else if (_c == QMetaObject::IndexOfMethod) {
        int *result = reinterpret_cast<int *>(_a[0]);
        {
            using _t = void (Server::*)(QMap<int,ClientData> );
            if (_t _q_method = &Server::initPlayers; *reinterpret_cast<_t *>(_a[1]) == _q_method) {
                *result = 0;
                return;
            }
        }
        {
            using _t = void (Server::*)(QMap<int,ClientData> );
            if (_t _q_method = &Server::updatePlayers; *reinterpret_cast<_t *>(_a[1]) == _q_method) {
                *result = 1;
                return;
            }
        }
    }
}

const QMetaObject *Server::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->dynamicMetaObject() : &staticMetaObject;
}

void *Server::qt_metacast(const char *_clname)
{
    if (!_clname) return nullptr;
    if (!strcmp(_clname, qt_meta_stringdata_CLASSServerENDCLASS.stringdata0))
        return static_cast<void*>(this);
    return QWidget::qt_metacast(_clname);
}

int Server::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QWidget::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 7)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 7;
    } else if (_c == QMetaObject::RegisterMethodArgumentMetaType) {
        if (_id < 7)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 7;
    }
    return _id;
}

// SIGNAL 0
void Server::initPlayers(QMap<int,ClientData> _t1)
{
    void *_a[] = { nullptr, const_cast<void*>(reinterpret_cast<const void*>(std::addressof(_t1))) };
    QMetaObject::activate(this, &staticMetaObject, 0, _a);
}

// SIGNAL 1
void Server::updatePlayers(QMap<int,ClientData> _t1)
{
    void *_a[] = { nullptr, const_cast<void*>(reinterpret_cast<const void*>(std::addressof(_t1))) };
    QMetaObject::activate(this, &staticMetaObject, 1, _a);
}
QT_WARNING_POP
