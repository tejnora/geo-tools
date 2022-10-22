#ifndef all_tran_polynomial_2d_h_
#define all_tran_polynomial_2d_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class quadratic_2d : public all_tran_base  
{
public:
quadratic_2d(const Mat<>& XXi,const Mat<>& xxi) : all_tran_base(XXi,xxi,12)
        {tt="quadratic_2d";}

quadratic_2d(const Vec<> hh) : all_tran_base(hh,12)  
        {tt="quadratic_2d";}

quadratic_2d() : all_tran_base(12)	{tt="quadratic_2d";}

//for the case that quadratic_2d is base function
quadratic_2d(const Vec<> hh, int pp) : all_tran_base(hh,pp)  {}        
quadratic_2d(int pp) : all_tran_base(pp)	{}
//for the case that quadratic_2d is base function

virtual ~quadratic_2d (){}

ostream& report_constants(ostream& out) const                           //virtual
        {out<<endl<<endl<<tt;
        out<<endl<<"The equation of "<<tt<<" is X = h1 + h2*x + h3*y + h4*x^2 + h5*xy + h6*y^2 and Y = h7 + h8*x + h9*y + h10*x^2 + h11*xy + h12*y^2";
        out<<endl<<"Computed parameters are (in order h1, h2, ..., h12)";
        return out;}

protected:

void ApproxSolution() 
{
	h.set_zero();
	//not necessary because of linear form
}

void fill_matrixes_quadratic_2d()
{
	//X0T and J		
	for(int i = 1; i <= r1; i++)
	{			
		X0T(i,1) = h(1) + h(2)*(*xi)(i,1) + h(3)*(*xi)(i,2) + h(4)*(*xi)(i,1)*(*xi)(i,1) + h(5)*(*xi)(i,1)*(*xi)(i,2) + h(6)*(*xi)(i,2)*(*xi)(i,2);
		X0T(i,2) = h(7) + h(8)*(*xi)(i,1) + h(9)*(*xi)(i,2) + h(10)*(*xi)(i,1)*(*xi)(i,1) + h(11)*(*xi)(i,1)*(*xi)(i,2) + h(12)*(*xi)(i,2)*(*xi)(i,2);
		
		J((i-1)*n+1,1) = 1; J((i-1)*n+1,2) = (*xi)(i,1); J((i-1)*n+1,3) = (*xi)(i,2); J((i-1)*n+1,4) = (*xi)(i,1)*(*xi)(i,1); 
		J((i-1)*n+1,5)= (*xi)(i,1)*(*xi)(i,2); J((i-1)*n+1,6)= (*xi)(i,2)*(*xi)(i,2);
		J((i-1)*n+2,7) = 1; J((i-1)*n+2,8) = (*xi)(i,1); J((i-1)*n+2,9) = (*xi)(i,2); J((i-1)*n+2,10) = (*xi)(i,1)*(*xi)(i,1); 
		J((i-1)*n+2,11)= (*xi)(i,1)*(*xi)(i,2); J((i-1)*n+2,12)= (*xi)(i,2)*(*xi)(i,2);
	}
}

void fill_matrixes()
{	
	fill_matrixes_quadratic_2d();
}

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,3);

		for(int i = 1; i <= num; i++)
		{			
			X(i,1) = h(1) + h(2)*x(i,1) + h(3)*x(i,2) + h(4)*x(i,1)*x(i,1) + h(5)*x(i,1)*x(i,2) + h(6)*x(i,2)*x(i,2);
			X(i,2) = h(7) + h(8)*x(i,1) + h(9)*x(i,2) + h(10)*x(i,1)*x(i,1) + h(11)*x(i,1)*x(i,2) + h(12)*x(i,2)*x(i,2);
		}			
		return X;
	}	
	else
	{
		throw at_exception("quadratic::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}			
};

class cubic_2d : public quadratic_2d  
{
public:
cubic_2d(const Mat<>& XXi,const Mat<>& xxi) : quadratic_2d(XXi,xxi)
        {tt="cubic_2d"; reset(20);}

cubic_2d(const Vec<> hh) : quadratic_2d(hh,20)  
        {tt="cubic_2d";}

cubic_2d() : quadratic_2d(20)	{tt="cubic_2d";}

//for the case of base function
cubic_2d(const Vec<> hh, int pp) : quadratic_2d(hh,pp)  {}    
cubic_2d(int pp) : quadratic_2d(pp)	{}
//for the case of base function

virtual ~cubic_2d (){}

ostream& report_constants(ostream& out) const                           //virtual
        {out<<endl<<endl<<tt;
        out<<endl<<"The equation of "<<tt<<" is X = h1 + h2*x + h3*y + h4*x^2 + h5*xy + h6*y^2 + h13*x^3 + h14*x*y^2 + h15*x^2*y + h16*y^3";
		out<<endl<<" and Y = h7 + h8*x + h9*y + h10*x^2 + h11*xy + h12*y^2 + h17*x^3 + h18*x*y^2 + h19*x^2*y + h20*y^3";
        out<<endl<<"Computed parameters are (in order h1, h2, ..., h20)";
        return out;}

protected:

//void ApproxSolution(){}  -define in quadratic_2d

void fill_matrixes_cubic_2d()
{			
	for(int i = 1; i <= r1; i++)
	{			
		X0T(i,1) += h(13)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1) + h(14)*(*xi)(i,1)*(*xi)(i,2)*(*xi)(i,2) + h(15)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,2)
			+ h(16)*(*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2);
		X0T(i,2) += h(17)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1) + h(18)*(*xi)(i,1)*(*xi)(i,2)*(*xi)(i,2) + h(19)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,2)
			+ h(20)*(*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2);

		J((i-1)*n+1,13) = (*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1); J((i-1)*n+1,14) = (*xi)(i,1)*(*xi)(i,2)*(*xi)(i,2); 
		J((i-1)*n+1,15) = (*xi)(i,1)*(*xi)(i,1)*(*xi)(i,2);	J((i-1)*n+1,16)= (*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2);
		J((i-1)*n+2,17) = (*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1); J((i-1)*n+2,18) = (*xi)(i,1)*(*xi)(i,2)*(*xi)(i,2); 
		J((i-1)*n+2,19) = (*xi)(i,1)*(*xi)(i,1)*(*xi)(i,2);	J((i-1)*n+2,20)= (*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2);
	}				
}

void fill_matrixes()
{	
	fill_matrixes_quadratic_2d();
	fill_matrixes_cubic_2d();
}

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,3);

		for(int i = 1; i <= num; i++)
		{			
			X(i,1) = h(1) + h(2)*x(i,1) + h(3)*x(i,2) + h(4)*x(i,1)*x(i,1) + h(5)*x(i,1)*x(i,2) + h(6)*x(i,2)*x(i,2);
			X(i,1) += h(13)*x(i,1)*x(i,1)*x(i,1) + h(14)*x(i,1)*x(i,2)*x(i,2) + h(15)*x(i,1)*x(i,1)*x(i,2) + h(16)*x(i,2)*x(i,2)*x(i,2);
			X(i,2) = h(7) + h(8)*x(i,1) + h(9)*x(i,2) + h(10)*x(i,1)*x(i,1) + h(11)*x(i,1)*x(i,2) + h(12)*x(i,2)*x(i,2);
			X(i,2) += h(17)*x(i,1)*x(i,1)*x(i,1) + h(18)*x(i,1)*x(i,2)*x(i,2) + h(19)*x(i,1)*x(i,1)*x(i,2) + h(20)*x(i,2)*x(i,2)*x(i,2);
		}			
		return X;
	}	
	else
	{
		throw at_exception("cubic::transform_points","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}

Mat<> transform_points_inversion(const Mat<>& X)
{
	if(is_solved)
	{
		int num = X.rows();
		Mat<> x(num,3);
		double dx,dy;
		for(int i = 1; i <= num; i++)	
		{
			x(i,1) = X(i,1);
			x(i,2) = X(i,2);
			x(i,3) = X(i,3);
			int j = 1;
			//kout<<'\n'<<"point i, number: "<<i<<", "<<X(i,3);
			for(; j <= 10; j++)
			{
				dx = X(i,1);
				dx -= h(1) + h(2)*x(i,1) + h(3)*x(i,2) + h(4)*x(i,1)*x(i,1) + h(5)*x(i,1)*x(i,2) + h(6)*x(i,2)*x(i,2);
				dx -= h(13)*x(i,1)*x(i,1)*x(i,1) + h(14)*x(i,1)*x(i,2)*x(i,2) + h(15)*x(i,1)*x(i,1)*x(i,2) + h(16)*x(i,2)*x(i,2)*x(i,2);
				x(i,1) = x(i,1) + dx;
				dy = X(i,2);
				dy -= h(7) + h(8)*x(i,1) + h(9)*x(i,2) + h(10)*x(i,1)*x(i,1) + h(11)*x(i,1)*x(i,2) + h(12)*x(i,2)*x(i,2);
				dy -= h(17)*x(i,1)*x(i,1)*x(i,1) + h(18)*x(i,1)*x(i,2)*x(i,2) + h(19)*x(i,1)*x(i,1)*x(i,2) + h(20)*x(i,2)*x(i,2)*x(i,2);
				x(i,2) = x(i,2) + dy;
				//kout<<'\n'<<"dx, dy: "<<dx<<", "<<dy;
				if( fabs(x(i,1)/inv_tran_limit) > fabs(dx) && fabs(x(i,2)/inv_tran_limit) > fabs(dy) ) break;				
			}
			if(j>10)	throw at_exception("cubic::transform_points_inversion","No conversion in inverse iteration");
		}
		return x;
	}	
	else
	{
		throw at_exception("cubic::transform_points_inversion","Transformation key is not solved yet. Call method \"solve()\" first");
	}		
}

};

class quartic_2d : public cubic_2d  
{
public:
quartic_2d(const Mat<>& XXi,const Mat<>& xxi) : cubic_2d(XXi,xxi)
        {tt="quartic_2d"; reset(30);}

quartic_2d(const Vec<> hh) : cubic_2d(hh,30)  
        {tt="quartic_2d";}

quartic_2d() : cubic_2d(30)	{tt="quartic_2d";}

virtual ~quartic_2d (){}

ostream& report_constants(ostream& out) const                           //virtual
        {out<<endl<<endl<<tt;
        out<<endl<<"The equation of "<<tt<<" is X = h1 + h2*x + h3*y + h4*x^2 + h5*xy + h6*y^2 + h13*x^3 + h14*x*y^2 + h15*x^2*y + h16*y^3";
		out<<" + h21*x^4 + h22*x*y^3 + h23*x^2*y^2 + h24*x^3*y + h25*y^4";
		out<<endl<<"and Y = h7 + h8*x + h9*y + h10*x^2 + h11*xy + h12*y^2 + h17*x^3 + h18*x*y^2 + h19*x^2*y + h20*y^3";
		out<<" + h26*x^4 + h27*x*y^3 + h28*x^2*y^2 + h29*x^3*y + h30*y^4";
        out<<endl<<"Computed parameters are (in order h1, h2, ..., h30)";
        return out;}

protected:

//void ApproxSolution(){}  -define in quadratic_2d

void fill_matrixes_quartic_2d()
{			
	for(int i = 1; i <= r1; i++)
	{			
		X0T(i,1) += h(21)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1) + h(22)*(*xi)(i,1)*(*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2) + 
		h(23)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,2)*(*xi)(i,2) + h(24)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,2) 
		+ h(25)*(*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2);
		X0T(i,2) += h(26)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1) + h(27)*(*xi)(i,1)*(*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2) + 
		h(28)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,2)*(*xi)(i,2) + h(29)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,2) 
		+ h(30)*(*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2);

		J((i-1)*n+1,21) = (*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1); J((i-1)*n+1,22) = (*xi)(i,1)*(*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2); 
		J((i-1)*n+1,23) = (*xi)(i,1)*(*xi)(i,1)*(*xi)(i,2)*(*xi)(i,2);	J((i-1)*n+1,24)= (*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,2);
		J((i-1)*n+1,25)= (*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2);
		J((i-1)*n+2,26) = (*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1); J((i-1)*n+2,27) = (*xi)(i,1)*(*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2); 
		J((i-1)*n+2,28) = (*xi)(i,1)*(*xi)(i,1)*(*xi)(i,2)*(*xi)(i,2);	J((i-1)*n+2,29)= (*xi)(i,1)*(*xi)(i,1)*(*xi)(i,1)*(*xi)(i,2);
		J((i-1)*n+2,30)= (*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2)*(*xi)(i,2);		
	}				
}

void fill_matrixes()
{	
	fill_matrixes_quadratic_2d();
	fill_matrixes_cubic_2d();
	fill_matrixes_quartic_2d();
}

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,3);

		for(int i = 1; i <= num; i++)
		{			
			X(i,1) = h(1) + h(2)*x(i,1) + h(3)*x(i,2) + h(4)*x(i,1)*x(i,1) + h(5)*x(i,1)*x(i,2) + h(6)*x(i,2)*x(i,2);
			X(i,1) += h(13)*x(i,1)*x(i,1)*x(i,1) + h(14)*x(i,1)*x(i,2)*x(i,2) + h(15)*x(i,1)*x(i,1)*x(i,2) + h(16)*x(i,2)*x(i,2)*x(i,2);
			X(i,1) += h(21)*x(i,1)*x(i,1)*x(i,1)*x(i,1) + h(22)*x(i,1)*x(i,2)*x(i,2)*x(i,2) + 
			h(23)*x(i,1)*x(i,1)*x(i,2)*x(i,2) + h(24)*x(i,1)*x(i,1)*x(i,1)*x(i,2) + h(25)*x(i,2)*x(i,2)*x(i,2)*x(i,2);

			X(i,2) = h(7) + h(8)*x(i,1) + h(9)*x(i,2) + h(10)*x(i,1)*x(i,1) + h(11)*x(i,1)*x(i,2) + h(12)*x(i,2)*x(i,2);
			X(i,2) += h(17)*x(i,1)*x(i,1)*x(i,1) + h(18)*x(i,1)*x(i,2)*x(i,2) + h(19)*x(i,1)*x(i,1)*x(i,2) + h(20)*x(i,2)*x(i,2)*x(i,2);
			X(i,2) += h(26)*x(i,1)*x(i,1)*x(i,1)*x(i,1) + h(27)*x(i,1)*x(i,2)*x(i,2)*x(i,2) + 
			h(28)*x(i,1)*x(i,1)*x(i,2)*x(i,2) + h(29)*x(i,1)*x(i,1)*x(i,1)*x(i,2) + h(30)*x(i,2)*x(i,2)*x(i,2)*x(i,2);
		}			
		return X;
	}	
	else
	{
		throw at_exception("quartic::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}		

};


}   // namespace all_tran
#endif