#ifndef all_tran_dlt_rd2_h_
#define all_tran_dlt_rd2_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//jeste provest kontrolu

//---------------------------------------------------------------------------

class dlt_rd2 : public all_tran_base  
{

 //in this class is changed the symbol of picture coordinates from xi to Xi and the symbol of spatial coordinates from Xi to xi

public:
dlt_rd2(const Mat<>& XXi,const Mat<>& xxi) : all_tran_base(XXi,xxi,16)  
        {tt="dlt_rd2";}

dlt_rd2(const Vec<> hh) : all_tran_base(hh,16)  
        {tt="dlt_rd2";}

dlt_rd2() : all_tran_base(16)  
		{tt="dlt_rd2";}

virtual ~dlt_rd2 (){}

ostream& report_constants(ostream& out) const                           //virtual
	{
		out<<endl<<"The standard elements of camera inner and outer orientation are: f, x0, y0, p, m, lambda, X0, Y0, Z0, R";		
		Mat<> a(3,1),b(3,1),c(3,1),R(3,3),hu(3,1),ha(3,3);		
		for(int i = 1; i<=3; i++)	
		{
			a(i,1) = h(i);
			b(i,1) = h(4+i);
			c(i,1) = h(8+i);
		}
		double f,x0,y0,p,m,lambda,X0,Y0,Z0;
		x0 = (trans(a)*c)(1,1)/(trans(c)*c)(1,1);
		y0 = (trans(b)*c)(1,1)/(trans(c)*c)(1,1);
		f = sqrt( (trans(a)*a)(1,1)/(trans(c)*c)(1,1)- x0*x0 ) ;
		p = sqrt((trans(c)*c)(1,1));
		for(int i = 1; i<=3; i++)	
		{
			R(i,1) = a(i,1); R(i,2) = b(i,1); R(i,3) = c(i,1);
		}
		m = (-1)*determinant(R)/(p*p*p*f*f);
		lambda = ( ((trans(a)*b)*(trans(c)*c))(1,1) - ((trans(a)*c)*(trans(b)*c))(1,1) ) / ( ((trans(a)*a)*(trans(c)*c))(1,1) - ((trans(a)*c)*(trans(a)*c))(1,1) );
		hu(1,1) = -h(4); hu(2,1) = -h(8); hu(3,1) = -1;		
		R.transpose();
		hu = inv(R)* hu;
		X0 = hu(1,1); Y0 = hu(2,1); Z0 = hu(3,1);
		ha(1,1) = m; ha(1,2) = 0; ha(1,3) = -m*x0;
		ha(2,1) = -lambda; ha(2,2) = 1; ha(2,3) = lambda*x0-m*y0;
		ha(3,1) = 0; ha(3,2) = 0; ha(3,3) = -m*f;
		R = 1/(p*m*f)*ha*R;
		out<<endl<<"f = "<<f;
		out<<endl<<"x0 = "<<x0;
		out<<endl<<"y0 = "<<y0;
		out<<endl<<"p = "<<p;
		out<<endl<<"m = "<<m;
		out<<endl<<"lambda = "<<lambda;
		out<<endl<<"X0 = "<<X0;
		out<<endl<<"Y0 = "<<Y0;
		out<<endl<<"Z0 = "<<Z0;
		out<<endl<<"R: "<<R;
			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"x = X*L1 + Y*L2 + Z*L3 + L4 / (X*L9 + Y*L10 + Z*L11 + 1)-(k1*r*r+k2*r*r*r*r+k3*r*r*r*r*r*r)*(x-x00)";
		out<<endl<<"y = X*L5 + Y*L6 + Z*L7 + L8 / (X*L9 + Y*L10 + Z*L11 + 1)-(k1*r*r+k2*r*r*r*r+k3*r*r*r*r*r*r)*(y-y00)";
        out<<endl<<"Computed parameters are (in order L1, L2, ..., L11, k1, k2, k3, x00, y00)";
        return out;
	}

protected:
//Mat<> Xiiter;
//double x0,y0;

void ApproxSolution() 
{
	h.set_zero();
	dlt_rd at((*Xi),(*xi));
	at.solve();
	Vec<> hh;
	hh = at.get_solution();	
	for(int i = 1; i <= 14; i++)	h(i) = hh(i);
	//approximate x00, y00
	Mat<> a(3,1),b(3,1),c(3,1);		
	for(int i = 1; i<=3; i++)	
	{
		a(i,1) = h(i);
		b(i,1) = h(4+i);
		c(i,1) = h(8+i);
	}	
	h(15) = (trans(a)*c)(1,1)/(trans(c)*c)(1,1);
	h(16) = (trans(b)*c)(1,1)/(trans(c)*c)(1,1);
}

void fill_matrixes_dlt_rd2()
{	
	//J a X0T	
	double A,B,C,r,rr,x,y;
	for(int i = 0; i < r1; i++)
	{
		x = (*Xi)(i+1,1)-h(15);
		y = (*Xi)(i+1,2)-h(16);
		r = sqrt( x*x + y*y );
		rr = h(12)*r*r + h(13)*r*r*r*r + h(14)*r*r*r*r*r*r;
		A = (*xi)(i+1,1)*h(1) + (*xi)(i+1,2)*h(2) + (*xi)(i+1,3)*h(3) + h(4);
		B = (*xi)(i+1,1)*h(5) + (*xi)(i+1,2)*h(6) + (*xi)(i+1,3)*h(7) + h(8);
		C = (*xi)(i+1,1)*h(9) + (*xi)(i+1,2)*h(10) + (*xi)(i+1,3)*h(11) + 1;		
		
		J(i*n+1,1) = (*xi)(i+1,1)/C; J(i*n+1,2)=(*xi)(i+1,2)/C; J(i*n+1,3)=(*xi)(i+1,3)/C; J(i*n+1,4)=1/C ; 
		J(i*n+1,9) = -A*(*xi)(i+1,1)/(C*C); J(i*n+1,10)= -A*(*xi)(i+1,2)/(C*C); J(i*n+1,11)= -A*(*xi)(i+1,3)/(C*C);
		J(i*n+1,12)= -((*Xi)(i+1,1)-h(15))*r*r; J(i*n+1,13)= -((*Xi)(i+1,1)-h(15))*r*r*r*r; J(i*n+1,14)= -((*Xi)(i+1,1)-h(15))*r*r*r*r*r*r;
		J(i*n+1,15) = 2*x*x*(h(12)+2*h(13)*r*r+3*h(14)*r*r*r*r) + rr;
		J(i*n+1,16) = 2*x*y*(h(12)+2*h(13)*r*r+3*h(14)*r*r*r*r);

		J(i*n+2,5)=(*xi)(i+1,1)/C; J(i*n+2,6)=(*xi)(i+1,2)/C; J(i*n+2,7)=(*xi)(i+1,3)/C; J(i*n+2,8)=1/C;
		J(i*n+2,9)= -B*(*xi)(i+1,1)/(C*C); J(i*n+2,10)= -B*(*xi)(i+1,2)/(C*C); J(i*n+2,11)= -B*(*xi)(i+1,3)/(C*C);
		J(i*n+2,12)= -((*Xi)(i+1,2)-h(16))*r*r; J(i*n+2,13)= -((*Xi)(i+1,2)-h(16))*r*r*r*r; J(i*n+2,14)= -((*Xi)(i+1,2)-h(16))*r*r*r*r*r*r;
		J(i*n+2,15) = 2*y*x*(h(12)+2*h(13)*r*r+3*h(14)*r*r*r*r);
		J(i*n+2,16) = 2*y*y*(h(12)+2*h(13)*r*r+3*h(14)*r*r*r*r) + rr;
				
		X0T(i+1,1) = A/C - rr * x;
		X0T(i+1,2) = B/C - rr * y;									
	}	
}

void fill_matrixes() {fill_matrixes_dlt_rd2();}

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,4);
		
		double r,rr;
				
		double A,B,C;
		for(int i = 1; i <= num; i++)
		{
			A = x(i,1)*h(1) + x(i,2)*h(2) + x(i,3)*h(3) + h(4);
			B = x(i,1)*h(5) + x(i,2)*h(6) + x(i,3)*h(7) + h(8);
			C = x(i,1)*h(9) + x(i,2)*h(10) + x(i,3)*h(11) + 1;

			X(i,1) = A/C;
			X(i,2) = B/C;	
			for(int j = 1; j<=4; j++)
			{
				r = sqrt( (X(i,1)-h(15))*(X(i,1)-h(15)) + (X(i,2)-h(16))*(X(i,2)-h(16)) );
				rr = h(12)*r*r + h(13)*r*r*r*r + h(14)*r*r*r*r*r*r;				
				X(i,1) = A/C - rr * (X(i,1)-h(15));
				X(i,2) = B/C - rr * (X(i,2)-h(16));						
			}
		}	
		return X;
	}	
	else
	{
		throw at_exception("dlt_rd2::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};

}   // namespace all_tran
#endif