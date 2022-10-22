// alltran_console.cpp : Defines the entry point for the console application.
//
#include "stdafx.h"

#include<fstream>

#define ITERATIONS  //writing of iteration to iterations.txt file
#include "alltran/alltran.h"

#include<string>

using namespace std;
using namespace GNU_gama;
using namespace all_tran;


int _tmain(int argc, _TCHAR* argv[])
{
	ifstream ink("input.txt");
	ifstream inp("local_points.txt");
	ofstream outk("key_report.txt");	
    string tt,type,p,con; // name of transformation, type of constructor, points, constants
    all_tran_base* at;
    Mat<> Xi,xi,x; //indentical points in global system (Xi) and local system (xi), points to transform
	Vec<> key,vconst;
    //BandMat<> S;
    //bool cm;    // covariant matrix	

	/* "input.txt" file structure	
	r1: "transformation_name" - name of transformation - see below	
	r2: "points" || "no_points" || "points_inv" - transform also non-identical points from "local_points.txt"
	r3: "constants" || " " - constants for transformation
	r3: constants data ... || " " - constants 
	r5: "global_points" || "key" - type of transformation constructor
	r6: data ...
	r?: "local_points"
	*/
	ink>>tt>>p>>con;
    try
	{
		//local_points		
		if(p == "points" || p == "points_inv")	
		{
            if(!inp)	throw at_exception("alltran.cpp","Local points file is missing!");
			read_points(x,inp);		
		}
		else if(p == "no_points"){}
		else	throw at_exception("alltran::main","unknown parameter");

		//constants
		if(con == "constants")
		{
			read_vector(vconst,ink);
			ink>>type;
		}
		else	type = con;		

		//type of transformation constructor		
		if(type == "global_points")	read_identical_points(Xi,xi,ink,tt);
		else if (type == "key")	read_vector(key,ink);
		else	throw at_exception("alltran::main","unknown type of transformation");				

		//transformation
		if (tt=="general_affine_linear") at = new general_affine_linear(Xi,xi); 
		else if (tt=="general_affine_m") at = new general_affine_m(Xi,xi);
		else if (tt=="affine_3d") {if(type != "key") at = new affine_3d(Xi,xi);else at = new affine_3d(key);}
		else if (tt=="similarity_3d") {if(type != "key") at = new similarity_3d(Xi,xi);else at = new similarity_3d(key);}
		else if (tt=="identity_3d") {if(type != "key") at = new identity_3d(Xi,xi);else at = new identity_3d(key);}
		else if (tt=="affine_2d") {if(type != "key") at = new affine_2d(Xi,xi);else at = new affine_2d(key);}
		else if (tt=="similarity_2d") {if(type != "key") at = new similarity_2d(Xi,xi);else at = new similarity_2d(key);}
		else if (tt=="identity_2d") {if(type != "key") at = new identity_2d(Xi,xi);else at = new identity_2d(key);}
		//else if (tt=="dlt_app") at = new dlt_app(Xi,xi);  //only approximation solution
		else if (tt=="dlt") {if(type != "key") at = new dlt(Xi,xi);else at = new dlt(key);}
		else if (tt=="dlt_rd") {if(type != "key") at = new dlt_rd(Xi,xi);else at = new dlt_rd(key);}
		else if (tt=="dlt_rd2") {if(type != "key") at = new dlt_rd2(Xi,xi);else at = new dlt_rd2(key);}
		//else if (tt=="dlt_2d_app") at = new dlt_2d_app(Xi,xi);	//only approximation solution
		else if (tt=="dlt_2d") {if(type != "key") at = new dlt_2d(Xi,xi);else at = new dlt_2d(key);}
		else if (tt=="dlt_2d_rd") {if(type != "key") at = new dlt_2d_rd(Xi,xi,vconst);else at = new dlt_2d_rd(key,vconst);}
		else if (tt=="dlt_2d_rd2") {if(type != "key") at = new dlt_2d_rd2(Xi,xi,vconst);else at = new dlt_2d_rd2(key);}
		else if (tt=="dlt_2d_rd_td") {if(type != "key") at = new dlt_2d_rd_td(Xi,xi,vconst);else at = new dlt_2d_rd_td(key,vconst);}
		else if (tt=="dlt_2d_cubic_2d") {if(type != "key") at = new dlt_2d_cubic_2d(Xi,xi);else at = new dlt_2d_cubic_2d(key);}
		else if (tt=="inv_dlt_2d_rd2") at = new inv_dlt_2d_rd2(key);
		else if (tt=="rd") {if(type != "key") at = new rd(Xi,xi,vconst);else at = new rd(key,vconst);}
		else if (tt=="rd2") {if(type != "key") at = new rd2(Xi,xi,vconst);else at = new rd2(key);}
		else if (tt=="rd_td") {if(type != "key") at = new rd_td(Xi,xi,vconst);else at = new rd_td(key,vconst);}
		else if (tt=="thin_plate_spline_2d") {if(type != "key") at = new thin_plate_spline_2d(Xi,xi,vconst);else at = new thin_plate_spline_2d(key,vconst);}
		else if (tt=="quadratic_2d") {if(type != "key") at = new quadratic_2d(Xi,xi);else at = new quadratic_2d(key);}
		else if (tt=="cubic_2d") {if(type != "key") at = new cubic_2d(Xi,xi);else at = new cubic_2d(key);}
		else if (tt=="quartic_2d") {if(type != "key") at = new quartic_2d(Xi,xi);else at = new quartic_2d(key);}
		else if (tt=="projective") {if(type != "key") at = new projective(Xi,xi,vconst);else at = new projective(key,vconst);}		
		else if (tt=="projective_inner") {if(type != "key") at = new projective_inner(Xi,xi,vconst);else at = new projective_inner(key,vconst);}
		else if (tt=="projective_x0y0") {if(type != "key") at = new projective_x0y0(Xi,xi,vconst);else at = new projective_x0y0(key,vconst);}
		else if (tt=="projective_f") {if(type != "key") at = new projective_f(Xi,xi,vconst);else at = new projective_f(key,vconst);}		
		else if (tt=="projective_rd2") {if(type != "key") at = new projective_rd2(Xi,xi,vconst);else at = new projective_rd2(key,vconst);}		
		else if (tt=="projective_x0y0_rd") {if(type != "key") at = new projective_x0y0_rd(Xi,xi,vconst);else at = new projective_x0y0_rd(key,vconst);}	
		else if (tt=="projective_x0y0_rd_td") {if(type != "key") at = new projective_x0y0_rd_td(Xi,xi,vconst);else at = new projective_x0y0_rd_td(key,vconst);}
		else if (tt=="projective_planar") {if(type != "key") at = new projective_planar(Xi,xi,vconst);else at = new projective_planar(key,vconst);}
		else if (tt=="projective_planar_x0y0") {if(type != "key") at = new projective_planar_x0y0(Xi,xi,vconst);else at = new projective_planar_x0y0(key,vconst);}
		else if (tt=="projective_double_inner") {if(type != "key") at = new projective_double_inner(Xi,xi,vconst);else at = new projective_double_inner(key,vconst);}
		else if (tt=="projective_double") {if(type != "key") at = new projective_double(Xi,xi,vconst);else at = new projective_double(key,vconst);}
		//composite transformation
		else if (tt=="dlt_plus_rd") {if(type != "key") at = new dlt_plus_rd(Xi,xi);}//else at = new dlt_plus_rd(key);}				
		else if (tt=="dlt_plus_rd2") {if(type != "key") at = new dlt_plus_rd2(Xi,xi);}//else at = new dlt_plus_rd2(key);}
		else if (tt=="dlt_plus_tps_2d") {if(type != "key") at = new dlt_plus_tps_2d(Xi,xi,vconst);}//else at = new dlt_plus_tps_2d(key);}		
		else if (tt=="dlt_2d_plus_tps_2d") {if(type != "key") at = new dlt_2d_plus_tps_2d(Xi,xi,vconst);}//else at = new dlt_2d_plus_tps_2d(key);}
		else if (tt=="dlt_2d_rd2_plus_tps_2d") {if(type != "key") at = new dlt_2d_rd2_plus_tps_2d(Xi,xi,vconst);}//else at = new dlt_2d_rd2_plus_tps_2d(key);}
		else if (tt=="dlt_2d_plus_rd") {if(type != "key") at = new dlt_2d_plus_rd(Xi,xi,vconst);}//else at = new dlt_2d_plus_rd(key);}				
		else if (tt=="dlt_2d_plus_rd2") {if(type != "key") at = new dlt_2d_plus_rd2(Xi,xi,vconst);}//else at = new dlt_2d_plus_rd2(key);}
		else if (tt=="dlt_2d_plus_quadratic_2d") {if(type != "key") at = new dlt_2d_plus_quadratic_2d(Xi,xi);}//else at = new dlt_2d_plus_quadratic_2d(key);}				
		else if (tt=="dlt_2d_plus_cubic_2d") {if(type != "key") at = new dlt_2d_plus_cubic_2d(Xi,xi);}//else at = new dlt_2d_plus_cubic_2d(key);}				
		else if (tt=="dlt_2d_plus_quartic_2d") {if(type != "key") at = new dlt_2d_plus_quartic_2d(Xi,xi);}//else at = new dlt_2d_plus_quartic_2d(key);}	
		else if (tt=="dlt_2d_rd_td_plus_cubic_2d") {if(type != "key") at = new dlt_2d_rd_td_plus_cubic_2d(Xi,xi,vconst);}
		else if (tt=="dlt_2d_plus_rd_td") {if(type != "key") at = new dlt_2d_plus_rd_td(Xi,xi,vconst);}//else at = new dlt_2d_plus_rd_td(key);}
		else if (tt=="projective_x0y0_plus_rd") {if(type != "key") at = new projective_x0y0_plus_rd(Xi,xi,vconst);}
		else if (tt=="projective_x0y0_plus_rd_td") {if(type != "key") at = new projective_x0y0_plus_rd_td(Xi,xi,vconst);}
		else if (tt=="projective_x0y0_plus_rd2") {if(type != "key") at = new projective_x0y0_plus_rd2(Xi,xi,vconst);}
		else if (tt=="projective_x0y0_plus_quadratic_2d") {if(type != "key") at = new projective_x0y0_plus_quadratic_2d(Xi,xi,vconst);}
		else if (tt=="projective_x0y0_plus_cubic_2d") {if(type != "key") at = new projective_x0y0_plus_cubic_2d(Xi,xi,vconst);}
		else if (tt=="projective_x0y0_plus_quartic_2d") {if(type != "key") at = new projective_x0y0_plus_quartic_2d(Xi,xi,vconst);}
		else if (tt=="projective_plus_cubic_2d") {if(type != "key") at = new projective_plus_cubic_2d(Xi,xi,vconst);}

	    else {outk<<endl<<"Unknown figure";return 0;}

		if(!at->solved())
		{
			at->solve();
			at->report(outk);
		}
		if(p == "points")
		{
			ofstream outp("transformed_points.txt");
			Mat<> X = at->transform_points(x);						
			outp.setf(ios_base::fixed);
			outp.precision(5);
			//outp<<X;
			int c = X.cols(),r = X.rows();
			for(int i = 1; i<=r;i++)
			{
				outp<<(int)X(i,c)<<' ';
				for(int j = 1; j<c;j++)	outp<<X(i,j)<<' ';
				outp<<endl;
			}
			outp.precision(6);
			outp.unsetf(ios_base::fixed);
		}
		if(p == "points_inv")
		{
			ofstream outp("transformed_points.txt");
			Mat<> X = at->transform_points_inversion(x);						
			outp.setf(ios_base::fixed);
			outp.precision(5);
			//outp<<X;
			int c = X.cols(),r = X.rows();
			for(int i = 1; i<=r;i++)
			{
				outp<<(int)X(i,c)<<' ';
				for(int j = 1; j<c;j++)	outp<<X(i,j)<<' ';
				outp<<endl;
			}
			outp.precision(6);
			outp.unsetf(ios_base::fixed);
		}
		delete at;
    }            
	catch(at_exception& e)
	{
		outk<<endl<<endl<<"AllTransform exception was called from (class::method): "<<e.location;
		outk<<endl<<"The desctription of exception: "<<e.description;
		outk<<endl<<"Has been called for transformation: "<<tt;
		delete at;
	}                
	catch(at_exception_gmv& e)
	{
		outk<<endl<<endl<<"AllTransform exception was called from (class::method): "<<e.location;
		outk<<endl<<"The desctription of exception: "<<e.description;
		outk<<endl<<"Has been called for transformation: "<<tt;					
		outk<<endl<<"GNU_gama exception: "<<e.ex.error<<"  "<<e.ex.description;				
		delete at;
	}
	catch(GNU_gama::Exception::matvec& exc)
	{
		outk<<endl<<endl<<"Has been called for transformation: "<<tt;
		outk<<endl<<"GNU_gama exception: "<<exc.error<<"  "<<exc.description;				
		delete at;
	}  

	return 0;
}

