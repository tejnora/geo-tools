#ifndef all_tran_inv_dlt_2d_rd2_h_
#define all_tran_inv_dlt_2d_rd2_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class inv_dlt_2d_rd2 : public all_tran_base
{

public:
inv_dlt_2d_rd2(const Vec<> hh) : all_tran_base(hh,13)  
        {tt="inv_dlt_2d_rd2";
		//parameters transformatin
		Vec<> th = h;
		double norm = th(2)*th(4)-th(1)*th(5);
		h(1) = (th(6)*th(8)-th(5))/norm;
		h(2) = (th(2)-th(3)*th(8))/norm;
		h(3) = (th(3)*th(5)-th(2)*th(6))/norm;
		h(4) = (th(4)-th(6)*th(7))/norm;
		h(5) = (th(3)*th(7)-th(1))/norm;
		h(6) = (th(1)*th(6)-th(3)*th(4))/norm;
		h(7) = (th(5)*th(7)-th(4)*th(8))/norm;
		h(8) = (th(1)*th(8)-th(2)*th(7))/norm;		
		}

virtual ~inv_dlt_2d_rd2 (){}

ostream& report_constants(ostream& out) const                           //virtual
	{			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"X = (x+dx)*L1 + (y+dy)*L2 + L3 / ((x+dx)*L7 + (y+dy)*L8 + 1); dx = (x-x0)*(k1*r*r+k2*r*r*r*r+k3*r*r*r*r*r*r)";
		out<<endl<<"Y = (x+dx)*L4 + (y+dy)*L5 + L6 / ((x+dx)*L7 + (y+dy)*L8 + 1); dy = (y-y0)*(k1*r*r+k2*r*r*r*r+k3*r*r*r*r*r*r)";
        out<<endl<<"Computed parameters are (in order L1, L2, ..., L8, k1, k2, k3, x0, y0)";
        return out;
	}

protected:

//pure virtual methods
void ApproxSolution(){}
void fill_matrixes() {}
//pure virtual methods

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);	
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,3);
		
		double A,B,C,r,rr,dx,dy;
		for(int i = 1; i <= num; i++)
		{
			r = sqrt( (x(i,1)-h(12))*(x(i,1)-h(12)) + (x(i,2)-h(13))*(x(i,2)-h(13)) );
			rr = h(9)*r*r + h(10)*r*r*r*r + h(11)*r*r*r*r*r*r;	
			dx = (x(i,1)-h(12)) * rr;
			dy = (x(i,2)-h(13)) * rr;

			A = (x(i,1)+dx)*h(1) + (x(i,2)+dy)*h(2) + h(3);
			B = (x(i,1)+dx)*h(4) + (x(i,2)+dy)*h(5) + h(6);
			C = (x(i,1)+dx)*h(7) + (x(i,2)+dy)*h(8) + 1;
			
			X(i,1) = A/C;
			X(i,2) = B/C;				
		}	
		return X;
	}	
	else
	{
		throw at_exception("inv_dlt_2d_rd2::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};

}   // namespace spat_fig
#endif