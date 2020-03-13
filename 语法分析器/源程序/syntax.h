#ifndef GRAMMAR_H
#define GRAMMAR_H
#include<iostream>
#include<string.h>
#include<fstream>
#include<cstdio>
#include<stack>
#include "STree.h"
#include "lexical.h"
#define Production_maxlength 6
#define Terminal_Sign_num 27
#define Nonterminal_Sign_num 40
#define Production_num 80
#define STACK_INIT_SIZE 50
#define STACK_INCREMENT 10
#define _CRT_SECURE_NO_WARNINGS
using namespace std;
struct Elem {
    int code;
    bool ter_flag;//是否为终结符
}; 
struct Production {
    int left;//左部符号在非终结符表数组中的位置
    bool left_terflag;
    int right[Production_maxlength];//右部符号在非终结符表/终结符表数组中的位置
    bool right_terflag[Production_maxlength];//右部符号是否为终结符
};

struct TableElem {
    Production pro;//预测分析表单个表项内容
    int ProNo;//存入的是第几个产生式
};

bool Analyze(STree* &root,FILE* fp,int *rowline,Grammar *temp,QString path);
class Grammar {
public:
    int proc_num;//产生式数目
    int tsign_num;//终止符号数目
    int ntsign_num;//非终结符标志数目
    Production proc[Production_num];
    int first_set[Nonterminal_Sign_num + 1][Terminal_Sign_num + 1];
    int follow_set[Nonterminal_Sign_num + 1][Terminal_Sign_num + 1];
    const char* terminal_sign[Terminal_Sign_num + 1];//终结符集合
    char* nonterminal_sign[Nonterminal_Sign_num + 1];//非终结符集合
    TableElem table[Nonterminal_Sign_num + 1][Terminal_Sign_num + 1];//预测分析表
    Grammar();
    void Init_Grammar(QString path);//从文件中读入文法并处理
    void Show_Grammar();
    void First();
    void Follow();
    void Show_First(int *rowline);
    void Show_Follow();
    void MTable();
    void Show_MTable();

    bool Exist(int A[Terminal_Sign_num + 1], int e);							//判断集合A中是否含有元素e
    bool Add_etoB(int e, int B[Terminal_Sign_num + 1]);							//把元素e加到集合B
    bool Add_AtoB(int A[Terminal_Sign_num + 1], int B[Terminal_Sign_num + 1],bool cancel_empty);	//把集合A加到集合B
    void fill_proc(int nter, int ter, int i);
    friend bool Analyze(STree* &root,FILE* fp,int *rowline,Grammar *temp,QString path);
public:
    vector <pair< char *,QString>>FIRST_VECTOR;//用于输出到tableview控件FIRST集存储vector
    vector <pair< char *,QString>>FOLLOW_VECTOR;//用于输出到tableview控件FOLLOW集存储vector
    vector <pair< QString,QString>>Grammar_Analysis;//用于输出到tableview控件语法规则集存储vector
    QString  MTableview[Nonterminal_Sign_num + 1][Terminal_Sign_num + 1];//用于输出到tableview控件的预测分析表集存储数组
};

#endif // COMPILER_H
