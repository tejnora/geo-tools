#include <iostream>
#include <iostream>
#include <fstream>

#include "gmatvec\mat.h"
#include "gmatvec\matvec.h"


//http://stackoverflow.com/questions/3776485/marshal-c-int-array-to-c-sharp

extern "C" _declspec(dllexport) GNU_gama::Mat<>* __stdcall Matrix_Create(int columns, int rows)
{
	GNU_gama::Mat<>* matrix = new GNU_gama::Mat<>();
	matrix->reset(rows,columns);
	return matrix;
}

extern "C" _declspec(dllexport) void __stdcall Matrix_Release(GNU_gama::Mat<>* THIS)
{
	delete THIS;
}

extern "C" _declspec(dllexport) void _stdcall Matrix_Set(GNU_gama::Mat<>* THIS, int rows, int columns, double value)
{
	(*THIS)(rows+1,columns+1)=value;
}

extern "C" _declspec(dllexport) double _stdcall Matrix_Get(GNU_gama::Mat<>* THIS, int rows,int columns)
{
	return (*THIS)(rows+1,columns+1);
}

extern "C" _declspec(dllexport) int _stdcall Matrix_Columns(GNU_gama::Mat<>* THIS)
{
	return THIS->cols();
}

extern "C" _declspec(dllexport) int _stdcall Matrix_Rows(GNU_gama::Mat<>* THIS)
{
	return THIS->rows();
}
