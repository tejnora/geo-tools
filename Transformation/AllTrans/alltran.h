// version 1.00

#ifndef alltran_h_
#define alltran_h_

#include<cmath>
#include<string>
#include<fstream>
#include<ctime>
#include<matvec/matvec.h>
#include<matvec/bandmat.h>
#include<matvec/pinv.h>
#include<matvec/svd.h>
#include<matvec/added_by_koska.h>

#define M_PI 3.1415926535897932384626433832795

#ifdef ITERATIONS
	std::ofstream kout("iterations.txt");
#endif

//#define DEBUG  //writing of iteration to debug.txt file
#ifdef DEBUG 
	std::ofstream dout("debug.txt");
#endif

#include<alltran/at_exception.h>
#include<alltran/all_tran_base.h>
#include<alltran/composite_tran_base.h>

#include<alltran/general_affine_multi.h> //multi dimensional
#include<alltran/affine_etc_3d.h>
#include<alltran/affine_etc_2d.h>
#include<alltran/dlt_app.h>
#include<alltran/dlt.h>
#include<alltran/dlt_rd.h>
#include<alltran/dlt_rd2.h>
#include<alltran/dlt_2d_app.h>
#include<alltran/dlt_2d.h>
#include<alltran/dlt_2d_rd.h>
#include<alltran/dlt_2d_rd2.h>
#include<alltran/dlt_2d_rd_td.h>
#include<alltran/dlt_2d_cubic_2d.h>
#include<alltran/inv_dlt_2d_rd2.h>
#include<alltran/rd.h>
#include<alltran/rd2.h>
#include<alltran/rd_td.h>
#include<alltran/thin_plate_spline_2d.h>
#include<alltran/polynomial_2d.h>
#include<alltran/projective.h>
#include<alltran/projective_something.h>
#include<alltran/projective_planar.h>
#include<alltran/projective_double.h>

#include<alltran/dlt_plus_rd.h>
#include<alltran/dlt_plus_rd2.h>
#include<alltran/dlt_plus_tps_2d.h>
#include<alltran/dlt_2d_plus_tps_2d.h>
#include<alltran/dlt_2d_rd2_plus_tps_2d.h>
#include<alltran/dlt_2d_plus_something.h>
#include<alltran/projective_x0y0_plus_something.h>

#endif     
