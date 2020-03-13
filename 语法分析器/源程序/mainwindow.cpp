#include "mainwindow.h"
#include "ui_mainwindow.h"
#include "lexical.h"
#include "STree.cpp"
#include "syntax.h"
#include <QString>
#include <queue>
#include "syntax.cpp"
#include <QAction>
#include <QMenuBar>
#include <QMessageBox>
#include <QStatusBar>
#include <QToolBar>
#include <QDebug>
#include <QTextEdit>
#include <QFileDialog>
#include <QDialog>
#include <QFile>
#include <QTextStream>
#include <QTreeWidget>
#include <QTreeWidgetItem>
#include <QScrollArea>
#include <QScrollBar>
#include <QLayout>
MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    QWidget* p = takeCentralWidget();
    if(p)
       delete p;
    setDockNestingEnabled(true);
    connect(ui->actionOpen, &QAction::triggered, this, &MainWindow::openFile);
    connect(ui->actionSave, &QAction::triggered, this, &MainWindow::saveFile);
    connect(ui->actionTextEdit, &QAction::triggered, this, &MainWindow::openTextEdit);
    connect(ui->actionSyntax_Analysis_Window, &QAction::triggered, this, &MainWindow::openSyntaxAnal);
    connect(ui->actionSyntax_Analysis, &QAction::triggered, this, &MainWindow::SyntaxAnalysis);
}

MainWindow::~MainWindow()
{
    delete ui;
}
FILE* MainWindow::FP(QString path){
    QString tempPath;
    for(int i=0;i<path.size();i++)
    {
        if(path[i]=='/')
        {
            tempPath.append('\\');
            tempPath.append('\\');
        }
        else
        {
            tempPath.append(path[i]);
        }
    }
    if(!path.isEmpty()) {

        char* PATH;
        QByteArray ba = tempPath.toLatin1(); // must
        PATH=ba.data();


        FILE *fp=fopen(PATH,"r");

        if (!fp) {

            return NULL;
        }
        else
            return fp;
    }
    return NULL;
}

void MainWindow::SyntaxAnalysis(){

    FILE *fp=FP(PATH);
    STREE = new STree();
    int rowline;
    Grammar getResult_FIRST;
    vector <pair<char*,QString>>RESULT_FIRST;
    vector <pair<char*,QString>>RESULT_FOLLOW;
    vector <pair<QString,QString>>RESULT_Grammar;
    if(!Analyze(STREE,fp,&rowline,&getResult_FIRST,path_grammar))
    {
        QMessageBox::about(NULL, "错误", "语法分析错误。");

    }
    //qDebug()<<path_grammar;
    RESULT_FIRST=getResult_FIRST.FIRST_VECTOR;
    RESULT_FOLLOW=getResult_FIRST.FOLLOW_VECTOR;
    RESULT_Grammar=getResult_FIRST.Grammar_Analysis;

    QDialog *Dialog_first=new QDialog;
    Dialog_first->setWindowTitle("FIRST集合");
    QTableWidget *tableView=new QTableWidget(Dialog_first);
    tableView->setColumnCount(2);
    tableView->setRowCount(rowline);
    tableView->resize(600, 600);
    tableView->setHorizontalHeaderLabels(QStringList()<<"非终结符"<<"FIRST成员");
    tableView->setItem(0,0,new QTableWidgetItem("item1"));
    tableView->horizontalHeader()->setSectionResizeMode(QHeaderView::Stretch);



    int row_first=0;
    for(vector<pair<char*,QString>>::const_iterator iter = RESULT_FIRST.cbegin(); iter != RESULT_FIRST.cend(); iter++,row_first++)
    {
        QTableWidgetItem *item0, *item1;
        item0 = new QTableWidgetItem;
        item1 = new QTableWidgetItem;
        QString txt = QString((*iter).first);
        item0->setText(txt);
        tableView->setItem(row_first, 0, item0);
        txt = QString((*iter).second);
        item1->setText(txt);
        tableView->setItem(row_first, 1, item1);
    }
    Dialog_first->setWindowIcon(QIcon(":/resource/tongji.ico"));
    Dialog_first->show();


    QDialog *Dialog_follow=new QDialog;
    Dialog_follow->setWindowTitle("FOLLOW集合");
    QTableWidget *tableView1=new QTableWidget(Dialog_follow);
    tableView1->setColumnCount(2);
    tableView1->setRowCount(rowline);
    tableView1->resize(600, 600);
    tableView1->setHorizontalHeaderLabels(QStringList()<<"非终结符"<<"FOLLOW成员");
    tableView1->setItem(0,0,new QTableWidgetItem("item1"));
    tableView1->horizontalHeader()->setSectionResizeMode(QHeaderView::Stretch);




    int row_follow=0;
    for(vector<pair<char*,QString>>::const_iterator iter = RESULT_FOLLOW.cbegin(); iter != RESULT_FOLLOW.cend(); iter++,row_follow++)
    {
        QTableWidgetItem *item0, *item1;
        item0 = new QTableWidgetItem;
        item1 = new QTableWidgetItem;
        QString txt = QString((*iter).first);
        item0->setText(txt);
        tableView1->setItem(row_follow, 0, item0);
        txt = QString((*iter).second);
        item1->setText(txt);
        tableView1->setItem(row_follow, 1, item1);
    }
    tableView1->horizontalHeader()->setSectionResizeMode(QHeaderView::Stretch);
    Dialog_follow->setWindowIcon(QIcon(":/resource/tongji.ico"));
    Dialog_follow->show();



    QDialog *Dialog_grammar=new QDialog;
    Dialog_grammar->setWindowTitle("Grammar规则");
    QTableWidget *tableView2=new QTableWidget(Dialog_grammar);
    tableView2->setColumnCount(2);
    tableView2->setRowCount(65);
    tableView2->resize(600, 600);
    tableView2->setHorizontalHeaderLabels(QStringList()<<"产生式左部"<<"产生式右部");
    tableView2->setItem(0,0,new QTableWidgetItem("item1"));
    tableView2->horizontalHeader()->setSectionResizeMode(QHeaderView::Stretch);
    int row_grammar=0;
    for(vector<pair<QString,QString>>::const_iterator iter = RESULT_Grammar.cbegin(); iter != RESULT_Grammar.cend(); iter++,row_grammar++)
    {
        QTableWidgetItem *item0, *item1;
        item0 = new QTableWidgetItem;
        item1 = new QTableWidgetItem;
        QString txt = QString((*iter).first);
        item0->setText(txt);
        tableView2->setItem(row_grammar, 0, item0);
        txt = QString((*iter).second);
        item1->setText(txt);
        tableView2->setItem(row_grammar, 1, item1);
    }
    tableView2->horizontalHeader()->setSectionResizeMode(QHeaderView::Stretch);
    Dialog_grammar->setWindowIcon(QIcon(":/resource/tongji.ico"));
    Dialog_grammar->show();




    QDialog *Dialog_mtable=new QDialog;
    Dialog_mtable->setWindowTitle("预测分析表");
    QTableWidget *tableView3=new QTableWidget(Dialog_mtable);
    tableView3->setColumnCount(Terminal_Sign_num);
    tableView3->setRowCount(Nonterminal_Sign_num-3);
    tableView3->resize(1000, 1000);
    int row_mtable,iter=0;
    for(int i = 0; i < Terminal_Sign_num; i++)
    {
        tableView3->setColumnWidth(i, 200);  //多一些空余控件，不然每列内容很挤
    }
    for(row_mtable=0;row_mtable<Nonterminal_Sign_num+1; row_mtable++)
    {
        for(iter=0;iter<Terminal_Sign_num+1;iter++)
        {

            QTableWidgetItem *item0;
            item0 = new QTableWidgetItem;
            QString txt =getResult_FIRST.MTableview[row_mtable][iter];
            item0->setText(txt);
            tableView3->setItem(row_mtable, iter, item0);
        }
    }
    Dialog_mtable->setWindowIcon(QIcon(":/resource/tongji.ico"));
    Dialog_mtable->show();




    QTreeWidget *STT=new QTreeWidget(ui->SyntaxAnalysisWin);
    QList<QTreeWidgetItem *> rootList;
    stack <STree*> ST;
    stack<QTreeWidgetItem *>QItem;
    STree *current;
    ST.push(STREE);
    QTreeWidgetItem *imageItem1 = new QTreeWidgetItem;   //添加第一个父节点
    imageItem1->setText(0,QString(STREE->getData()) );
    QItem.push(imageItem1);
    rootList.append(imageItem1);
    while (!ST.empty())
    {
        current = ST.top();
        ST.pop();
        QTreeWidgetItem * tempItem=QItem.top();
        QItem.pop();
        for (int j=0; j < current->getchild_num(); j++)
        {
            ST.push(current->getChild(j));
            QTreeWidgetItem *newItem = new QTreeWidgetItem;   //添加第一个父节点
            newItem->setText(0,tr(current->getChild(j)->getData()));
            tempItem->addChild(newItem);
            QItem.push(newItem);
        }
    }
    QScrollArea *scrollArea = new QScrollArea;
    scrollArea->setBackgroundRole(QPalette::Dark);
    scrollArea->setWidget(STT);
    scrollArea->setAlignment(Qt::AlignCenter);  // 居中对齐
    scrollArea->setWidgetResizable(true);  // 自动调整大小
    STT->setHeaderLabel(tr("语法分析树"));
    STT->insertTopLevelItems(0,rootList);  //将结点插入部件中
    STT->expandAll(); //全部展开
    ui->SyntaxAnalysisWin->setWidget(scrollArea);
}

void MainWindow::openTextEdit(){
    ui->TextEdit->show();
}

void MainWindow::openSyntaxAnal(){
    ui->SyntaxAnalysisWin->show();
}

void MainWindow::openFile()
{
    QString path = QFileDialog::getOpenFileName(this,
                                                tr("Open File"),
                                                ".",
                                                tr("Text Files(*.txt);;Cpp Files(*.cpp);;Header Files(*.h)"));
    if(!path.isEmpty()) {
        QFile file(path);
        PATH=path;
        if (!file.open(QIODevice::ReadOnly | QIODevice::Text)) {
            QMessageBox::warning(this, tr("Read File"),
                                 tr("Cannot open file:\n%1").arg(path));
            return;
        }
        QTextStream in(&file);
        TextStr = new QTextStream(&file);
        ui->textEdit->setText(in.readAll());
        file.close();
    } else {
        QMessageBox::warning(this, tr("Path"),
                             tr("You did not select any file."));
    }
}

void MainWindow::saveFile()
{

    QString path=PATH;
    if(!path.isEmpty()) {
        QFile file(path);
        if (!file.open(QIODevice::WriteOnly | QIODevice::Text)) {
            QMessageBox::warning(this, tr("Write File"),
                                       tr("Cannot open file:\n%1").arg(path));
            return;
        }
        QTextStream out(&file);
        out << ui->textEdit->toPlainText();
        file.close();
    } else {
        QMessageBox::warning(this, tr("Path"),
                             tr("You did not select any file."));
    }
}
