#include <iostream>
#include <fstream>
#include <iostream>

#include "gmatvec\mat.h"
#include "gmatvec\matvec.h"
#include "gmatvec\bandmat.h"
#include "gmatvec\svd.h"
#include "AllTrans\all_tran_base.h"
#include "AllTrans\affine_etc_2d.h"

extern "C" _declspec(dllexport) void _stdcall Identity2D_Release(all_tran::all_tran_base* THIS)
{
	delete THIS;
}

extern "C" _declspec(dllexport) bool _stdcall Identity2D_Solved(all_tran::all_tran_base* THIS)
{
	return THIS->solved();
}

extern "C" _declspec(dllexport) void _stdcall Identity2D_Solve(all_tran::all_tran_base* THIS)
{
	THIS->solve();
}

extern "C" _declspec(dllexport) GNU_gama::Mat<>* Identity2D_TransformPoints(all_tran::all_tran_base* THIS, GNU_gama::Mat<>* localPoints)
{
	GNU_gama::Mat<>* resultMatrix=new GNU_gama::Mat<>();
	*resultMatrix=THIS->transform_points(*localPoints);
	return resultMatrix;
}

extern "C" _declspec(dllexport) Report* Identity2D_GetReport(all_tran::all_tran_base* THIS)
{
	Report* report=new Report();
	*report=THIS->get_report();
	return report;
}

void Identity2D_PrintMatrix(std::ofstream& outp, GNU_gama::Mat<>& matrix)
{
	outp.setf(std::ios_base::fixed);
	outp.precision(5);
	int c = matrix.cols(),r = matrix.rows();
	for(int i = 1; i<=r;i++)
	{
		outp<<(int)matrix(i,c)<<' ';
		for(int j = 1; j<c;j++)	
		{
			double value=matrix(i,j);
			outp<<value<<' ';
		}
		outp<<std::endl;
	}
	outp.precision(6);
	outp.unsetf(std::ios_base::fixed);
}
