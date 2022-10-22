#ifndef all_tran_dlt_rd_h_
#define all_tran_dlt_rd_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class dlt_rd : public all_tran_base  
{

 //in this class is changed the symbol of picture coordinates from xi to Xi and the symbol of spatial coordinates from Xi to xi

public:
dlt_rd(const Mat<>& XXi,const Mat<>& xxi) : all_tran_base(XXi,xxi,14)  
        {tt="dlt_rd";}

dlt_rd(const Vec<> hh) : all_tran_base(hh,14)  
        {tt="dlt_rd";}

dlt_rd() : all_tran_base(14)  
        {tt="dlt_rd";}

virtual ~dlt_rd (){}

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
		out<<endl<<"x = X*L1 + Y*L2 + Z*L3 + L4 / (X*L9 + Y*L10 + Z*L11 + 1)-(x-x0)*(k1*r*r+k2*r*r*r*r+k3*r*r*r*r*r*r)";
		out<<endl<<"y = X*L5 + Y*L6 + Z*L7 + L8 / (X*L9 + Y*L10 + Z*L11 + 1)-(y-y0)*(k1*r*r+k2*r*r*r*r+k3*r*r*r*r*r*r)";
        out<<endl<<"Computed parameters are (in order L1, L2, ..., L11, k1, k2, k3)";
        return out;
	}

protected:
//Mat<> Xiiter;
//double x0,y0;

void ApproxSolution() 
{
	h.set_zero();
	dlt at((*Xi),(*xi));
	at.solve();
	Vec<> hh(11);
	hh = at.get_solution();	
	for(int i = 1; i <= 11; i++)	h(i) = hh(i);	
}

void fill_matrixes_dlt_rd()
{	
	//J a X0T	
	double x0,y0;
	Mat<> a(3,1),b(3,1),c(3,1);
	for(int i = 1; i<=3; i++)	
		{
			a(i,1) = h(i);
			b(i,1) = h(4+i);
			c(i,1) = h(8+i);
		}
	x0 = (trans(a)*c)(1,1)/(trans(c)*c)(1,1);
	y0 = (trans(b)*c)(1,1)/(trans(c)*c)(1,1);
		
	double A,B,C,r,rr,x,y;
	for(int i = 0; i < r1; i++)
	{
		x = (*Xi)(i+1,1)-x0;
		y = (*Xi)(i+1,2)-y0;
		r = sqrt( x*x + y*y );
		rr = h(12)*r*r + h(13)*r*r*r*r + h(14)*r*r*r*r*r*r;
		A = (*xi)(i+1,1)*h(1) + (*xi)(i+1,2)*h(2) + (*xi)(i+1,3)*h(3) + h(4);
		B = (*xi)(i+1,1)*h(5) + (*xi)(i+1,2)*h(6) + (*xi)(i+1,3)*h(7) + h(8);
		C = (*xi)(i+1,1)*h(9) + (*xi)(i+1,2)*h(10) + (*xi)(i+1,3)*h(11) + 1;		
		
		J(i*n+1,1) = (*xi)(i+1,1)/C; J(i*n+1,2)=(*xi)(i+1,2)/C; J(i*n+1,3)=(*xi)(i+1,3)/C; J(i*n+1,4)=1/C ; 
		J(i*n+1,9) = -A*(*xi)(i+1,1)/(C*C); J(i*n+1,10)= -A*(*xi)(i+1,2)/(C*C); J(i*n+1,11)= -A*(*xi)(i+1,3)/(C*C);
		J(i*n+1,12)= -x*r*r; J(i*n+1,13)= -x*r*r*r*r; J(i*n+1,14)= -x*r*r*r*r*r*r;

		J(i*n+2,5)=(*xi)(i+1,1)/C; J(i*n+2,6)=(*xi)(i+1,2)/C; J(i*n+2,7)=(*xi)(i+1,3)/C; J(i*n+2,8)=1/C;
		J(i*n+2,9)= -B*(*xi)(i+1,1)/(C*C); J(i*n+2,10)= -B*(*xi)(i+1,2)/(C*C); J(i*n+2,11)= -B*(*xi)(i+1,3)/(C*C);
		J(i*n+2,12)= -y*r*r; J(i*n+2,13)= -y*r*r*r*r; J(i*n+2,14)= -y*r*r*r*r*r*r;
		
		/*
		//correction of objective
		// if the image is corrected the process should be iterative
		X0T(i+1,1) = A/C;
		X0T(i+1,2) = B/C;
		
		r = sqrt( (X0T(i+1,1)-x0)*(X0T(i+1,1)-x0) + (X0T(i+1,2)-y0)*(X0T(i+1,2)-y0) );
		rr = h(12)*r*r + h(13)*r*r*r*r + h(14)*r*r*r*r*r*r;
		X0T(i+1,1) = A/C + rr * (X0T(i+1,1)-x0);
		X0T(i+1,2) = B/C + rr * (X0T(i+1,2)-y0);
		*/
				
		//correction of CCD chip
		X0T(i+1,1) = A/C - x*rr;
		X0T(i+1,2) = B/C - y*rr;				
	}	
}

void fill_matrixes() {fill_matrixes_dlt_rd();}

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,4);
		
		double x0,y0,r,rr;
		Mat<> a(3,1),b(3,1),c(3,1);
		for(int i = 1; i<=3; i++)	
			{
				a(i,1) = h(i);
				b(i,1) = h(4+i);
				c(i,1) = h(8+i);
			}
		x0 = (trans(a)*c)(1,1)/(trans(c)*c)(1,1);
		y0 = (trans(b)*c)(1,1)/(trans(c)*c)(1,1);	
		
		double A,B,C;
		for(int i = 1; i <= num; i++)
		{
			A = x(i,1)*h(1) + x(i,2)*h(2) + x(i,3)*h(3) + h(4);
			B = x(i,1)*h(5) + x(i,2)*h(6) + x(i,3)*h(7) + h(8);
			C = x(i,1)*h(9) + x(i,2)*h(10) + x(i,3)*h(11) + 1;

			/*
			//correction of objective
			X(i,1) = A/C;
			X(i,2) = B/C;
			
			r = sqrt( (X(i,1)-x0)*(X(i,1)-x0) + (X(i,2)-y0)*(X(i,2)-y0) );
			rr = h(12)*r*r + h(13)*r*r*r*r + h(14)*r*r*r*r*r*r;					
			X(i,1) = A/C + rr * (X(i,1)-x0);
			X(i,2) = B/C + rr * (X(i,2)-y0);
			*/
						
			//correction of CCD chip
			X(i,1) = A/C;
			X(i,2) = B/C;
			for(int j = 1; j<=3; j++)
			{
				r = sqrt( (X(i,1)-x0)*(X(i,1)-x0) + (X(i,2)-y0)*(X(i,2)-y0) );
				rr = h(12)*r*r + h(13)*r*r*r*r + h(14)*r*r*r*r*r*r;					
				X(i,1) = A/C - rr * (X(i,1)-x0);
				X(i,2) = B/C - rr * (X(i,2)-y0);			
			}			
		}	
		return X;
	}	
	else
	{
		throw at_exception("dlt_rd::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};

}   // namespace all_tran
#endif