#ifndef all_tran_rd2_h_
#define all_tran_rd2_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class rd2 : public all_tran_base  
{
public:
rd2(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : all_tran_base(XXi,xxi,5)  
        {tt="rd2";q=0; c=2; x0=v(1); y0=v(2);}

rd2(const Vec<> hh) : all_tran_base(hh,5)  
	{tt="rd2"; x0=h(12); y0=h(13); c=2;}

//for the case of composite transformation
rd2() : all_tran_base(5)	{tt="rd2";c=2;}

void reset(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v)
{
	x0=v(1); y0=v(2);
	all_tran_base::reset(XXi,xxi);
}
//for the case of composite transformation

virtual ~rd2 (){}

ostream& report_constants(ostream& out) const                           //virtual
	{			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"X = x-(X-x0)*(k1*r*r+k2*r*r*r*r+k3*r*r*r*r*r*r)";
		out<<endl<<"Y = y-(Y-y0)*(k1*r*r+k2*r*r*r*r+k3*r*r*r*r*r*r)";
        out<<endl<<"Computed parameters are (in order k1, k2, k3, x0, y0)";
        return out;
	}

protected:
double x0,y0;

void ApproxSolution() 
{
	h.set_zero();	
	h(4)=x0; h(5)=y0;
}

void fill_matrixes_rd2()
{	
	//J a X0T			
	double r,rr,x,y;
	for(int i = 0; i < r1; i++)
	{
		x = (*Xi)(i+1,1)-h(4);
		y = (*Xi)(i+1,2)-h(5);
		r = sqrt( x*x + y*y );		
		rr = h(1)*r*r + h(2)*r*r*r*r + h(3)*r*r*r*r*r*r;				
				
		J(i*n+1,1)= -x*r*r;
		J(i*n+1,2)= -x*r*r*r*r;
		J(i*n+1,3)= -x*r*r*r*r*r*r;
		J(i*n+1,4) = 2*x*x*(h(1)+2*h(2)*r*r+3*h(3)*r*r*r*r) + rr;
		J(i*n+1,5) = 2*x*y*(h(1)+2*h(2)*r*r+3*h(3)*r*r*r*r);

		J(i*n+2,1)= -y*r*r; 
		J(i*n+2,2)= -y*r*r*r*r; 
		J(i*n+2,3)= -y*r*r*r*r*r*r;
		J(i*n+2,4) = 2*x*y*(h(1)+2*h(2)*r*r+3*h(3)*r*r*r*r);
		J(i*n+2,5) = 2*y*y*(h(1)+2*h(2)*r*r+3*h(3)*r*r*r*r) + rr;
		
		X0T(i+1,1) = (*xi)(i+1,1) - x * rr;
		X0T(i+1,2) = (*xi)(i+1,2) - y * rr;
	}	
}

void fill_matrixes() {fill_matrixes_rd2();}

public:
Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,3);
				
		double r,rr,xx,yy;
		for(int i = 1; i <= num; i++)
		{
			X(i,1) = x(i,1);
			X(i,2) = x(i,2);
			for(int j = 1; j <= 4; j++)
			{
				xx = X(i,1)-h(4);
				yy = X(i,2)-h(5);
				r = sqrt( xx*xx + yy*yy );
				rr = h(1)*r*r + h(2)*r*r*r*r + h(3)*r*r*r*r*r*r;	
						
				X(i,1) = x(i,1) - xx * rr ;
				X(i,2) = x(i,2) - yy * rr ;				
			}		
		}	
		return X;
	}	
	else
	{
		throw at_exception("rd2::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};

}   // namespace all_tran
#endif