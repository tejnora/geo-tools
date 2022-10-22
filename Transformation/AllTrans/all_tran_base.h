#ifndef all_tran_base_h_
#define all_tran_base_h_

#include <string>
#include "at_exception.h"
#include "../gmatvec/matvec.h"
#include "../ReportExport.h"

#define M_PI       3.14159265358979323846

namespace all_tran {

using namespace std;
using namespace GNU_gama;

class all_tran_base   
{

public:

std::ostream& report(std::ostream& out) const;
Report* get_report()const;
GNU_gama::Mat<> create_correlation_matrix()const;
virtual void solve()   
	{
		if(!is_solved)
		{
			if( (r1*n+q) >= p)
			{                        						
				InitMat();
				ApproxSolution();    

			#ifdef ITERATIONS
				kout.precision(8);
				kout<<endl<<endl<<tt;
				kout<<endl<<"iter."<<" "<<"         sig0"<<" "<<"       sig0cont";
				for(int i=1;i<=p;i++)   {kout<<" "<<"   parameter"; kout.width(3); kout.fill('0');kout<<i;}
				kout.fill(' ');
			#endif
		        
				int maxnofc=200;nofc=0;      //maximal number_of_calculation, number_of_calculation
				do
					{
						nofc++;                                
						fill_matrixes();                                
						adjustment();
					}
				while(verify_solution()&&nofc<maxnofc);
				if(nofc==maxnofc)
				{							
					throw at_exception("all_trans_base::solve",
					"Bad approximate solution, The solution couldn't be found, the maximum number of iteration was exceeded");
				}                        
				is_solved=true;						
			}
			else throw at_exception("all_tran_base::solve","There is no enough points to compute transformation key");	
		}
		else throw at_exception("all_tran_base::solve","Transformation key is already solved");	
	}

Vec<> get_solution() const
{
	if(is_solved)	return h;
	else	throw at_exception("all_tran_base::get_solution","Transformation key has't been solved yet. Call method \"solve()\" first");	
}

bool solved() const {return is_solved;}	
int get_p()	const {return p;}//p == number_of_variables
int get_c()	const {return c;}//c == number_of_constants
int get_q()	const {return q;}
double get_sig0()	/*const*/ {return sig0;}
string get_tt()	const {return tt;}

public:
int nofc;

protected:
const Mat<>* Xi;
const Mat<>* xi;
BandMat<> P,P_sqrt;
int p,q;	//number of variables, number of constraints 
int n,r1,c;        //dimension, number of identical points, number of constants
bool is_solved;
Vec<> h,dX,dh,v,b;
Mat<> X0T,J,B;
double sig0,w_of_constraints;
string tt; //type_of_transformation
int nullity;
double condition;
bool gauss;
Mat<> covariance;
struct corij
{
	double cor;
	int i;
	int j;
};
corij max_cor;
double test;
double inv_tran_limit;

all_tran_base(const Mat<>& XXi,const Mat<>& xxi, int pp)
:Xi(&XXi),xi(&xxi),p(pp),is_solved(false),n(XXi.cols()-1),r1(XXi.rows()),w_of_constraints(1e9),q(0),c(0),inv_tran_limit(1e6)
{
	h.reset(p);
	dh.reset(p);
}

all_tran_base(const Vec<> hh, int pp)  : p(pp),inv_tran_limit(1e6) //,q(0),c(0)?
{
	if(hh.dim() == pp)
	{
		h = hh;
		is_solved = true;
	}
	else	throw at_exception("all_trans_base::all_tran_base",	"The number of given values isn't in correspondence with the number of variables");		
}

// for the case of composite transformation: dlt_2d, dlt_2d_rd2, thin_plate_spline_2d, quadratic_2d, cubic_2d, quartic_2d
all_tran_base(int pp) : p(pp), is_solved(false),w_of_constraints(1e9),q(0),c(0),inv_tran_limit(1e6) {}

public:
void reset(const Mat<>& XXi,const Mat<>& xxi) 
{
	// "p", "q" and "c" must be defined before in constructors
	Xi = &XXi;
	xi = &xxi;
	is_solved = false;
	n = XXi.cols()-1;
	r1 = XXi.rows();
	w_of_constraints = 1e9;
	h.reset(p);
	dh.reset(p);
}

void reset(const Vec<> hh)  
{
	if(hh.dim() == p)
	{
		h = hh;
		is_solved = true;
	}
	else	throw at_exception("all_trans_base::all_tran_base",	"The number of given values isn't in correspondence with the number of variables");	
}

//only for cubic_2d and quartic_2d in polynomial_2d.h
void reset(int ppp)	
{
	p = ppp;
	h.reset(p);
	dh.reset(p);
}

//because of transformation with constants, constants doesn't exists in global level
virtual void reset(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v){}
virtual void reset(const Vec<> hh,const Vec<> v){}
//because of transformation with constants

virtual ~all_tran_base (){}

private:                 
void InitMat();
void adjustment();
bool verify_solution();
void max_correlation(Mat<>& cov);
ostream& out_correlation(ostream& out) const;

protected:
virtual void ApproxSolution()=0;
virtual void fill_matrixes()=0;

public:
virtual ostream& report_constants(ostream& out) const=0;
virtual Mat<> transform_points(const Mat<>& x)=0;
virtual Mat<> transform_points_inversion(const Mat<>& X)
{
	throw at_exception("all_trans_base::all_tran_base",	"The method \"transform_points_inversion\" is not defined");		
}

};

void all_tran_base::max_correlation(Mat<>& cov)
{
	double cor;
	double aa;
	max_cor.cor = 0;
	int n = cov.rows();
	for(int i = 1; i<=n; i++)
		for(int j = (i+1); j<=n; j++)	
		{	
			aa = sqrt(cov(i,i)*cov(j,j));
			if(aa != 0) cor = cov(i,j)/aa;
			else cor = 0;
			if(fabs(cor) > max_cor.cor) 
			{
				max_cor.cor = fabs(cor);			
				max_cor.i = i;
				max_cor.j = j;
			}
		}	
}

ostream& all_tran_base::out_correlation(ostream& kout) const
{
		kout.setf(ios_base::fixed);
		kout.precision(4);
		double cor;
		kout<<'\n';
		for(int i = 1; i<=p; i++)
		{			
			for(int j = 1; j<=p; j++)	
			{
				if(	covariance(i,i)==0 || covariance(j,j)==0 )	
					if(	covariance(i,j)==0 )	cor = 0;
					else	cor = 1;					
				else	cor = covariance(i,j)/sqrt(covariance(i,i)*covariance(j,j));								
				kout.width(7);kout<<cor<<' ';				
			}			
			kout<<'\n';
		}
		kout.unsetf(ios_base::fixed);
		return kout;
}

bool all_tran_base::verify_solution()
{
	bool a = false;
	for(int i=1;i<=p;i++)
		if ( fabs(h(i)/1e6) < fabs(dh(i)) ) a = true;	//six valid digits
	return a;
}

/*
bool all_tran_base::verify_solution()
{
	bool a = false;
	if ( (sig0/1e6) < fabs(sig0-sig0_old) ) a = true;
	sig0_old = sig0;
	return a;
}
*/

// reseni pouzitelne pouze pro hledane parametry velikosti jednotek
/*
bool all_tran_base::verify_solution()
{
	bool a=false;
	double sum=0;
	for(int i=1;i<=p;i++)
			//a+=!((sqrt(Sh(i,i))/100)>(abs(dh(i))));
			sum+=(dh(i)*dh(i));
	sum=sqrt(sum);        
	if(sum>1e-5) a=true;
	return a;
}
*/

inline void all_tran_base::InitMat()
        {
			dX.reset(n*r1);
			X0T.reset(r1,n);X0T.set_zero();
			B.reset(p,q);B.set_zero();
			b.reset(q);b.set_zero();
			J.reset(n*r1,p);J.set_zero();			
			P.reset(n*r1,0);P.set_all(1);			
			P_sqrt = P;			
		}

std::ostream& all_tran_base::report(std::ostream& out) const
        {out<<"------------------------------------------------------------------------------------------------------------";
        if(!is_solved)
			throw at_exception("all_tran_base::report","The transformation key hasn't been solved yet");
		//out.setf(ios_base::fixed);
		const int stdp = 10;
		out.precision(stdp);	

        report_constants(out);
        out<<endl<<"(Numbers smaller than 1e-50 will be displayed as "<<'"'<<'0'<<'"'<<')'<<endl;
        double limit=1e-50; //1e-17		
		
        if((r1*n+q)>=p)
        {
            for(int i=1;i<=p;i++)
			{
				if(!(abs(h(i))<limit)) out<<endl<<h(i)<<"  "; else out<<endl<<'0'<<"  ";
			}                        
            out<<endl<<endl<<"Number of identical points is: "<<r1;            
            out<<endl<<"Standard deviation a posteriori is (sqrt([vv]/number_of_redundant_points)): "<<sig0; 
			out<<endl<<"Standard deviation a posteriori in coordinates is (sqrt([vv]/number_of_redundant_coordinates)): "<<(sig0/sqrt((double)(n)));
            out<<endl<<"Number of iterations is: "<<nofc;            
            }
        else    {for(int i=1;i<=p;i++)
                        {if(!(abs(h(i))<limit)) out<<endl<<h(i)<<"  "; else out<<endl<<'0'<<"  ";}
                out<<endl<<endl<<"Number of identical points is: "<<r1;
                out<<endl<<"There is only necessary number of points, standard deviation can't be computed.";}

		//stability report
		if(covariance.cols() != 0)	//constraint for non-composite transformation
		{
			out<<'\n'<<"Used solution algorithm: ";
			if(gauss)	out<<"Gauss-Jordan elimination";
			else	
			{
				out<<"SVD";
				if(nullity != 0)	out<<'\n'<<"Coefficient matrix of project equations is ill-conditioned or singular, the number of matrix defect is: "<<nullity;			
			}			
			if(q!=0)	out<<'\n'<<"The sum of normal equations absolute residuals: "<<test;
			out.setf(ios_base::fixed);	out.precision(0);
			out<<'\n'<<"The condition number is: "<<condition;
			out.precision(stdp);	out.unsetf(ios_base::fixed);			
			out<<'\n'<<"Maximal correlation is: "<<max_cor.cor<<" between parameters: "<<max_cor.i<<' '<<max_cor.j;
			out<<'\n'<<"Correlation matrix:"<<'\n';
			out_correlation(out);
		}
		//stability report
		
		//residuals
		out.setf(ios_base::fixed);
		out.precision(5);
		out<<endl<<endl<<"Residuals: "<<endl;
        for(int i = 0; i < r1; i++)
		{
			out<<(int)(*Xi)(i+1,n+1)<<' ';
			for(int j = 1; j<=n; j++)	{out.width(8);out<<v(i*n+j)<<' ';}
			out<<endl;
		}

		out.precision(6);
		out.unsetf(ios_base::fixed);
        out<<endl<<endl;
        return out;}

Report* all_tran_base::get_report() const
{
	if(!is_solved)
		throw at_exception("all_tran_base::report","The transformation key hasn't been solved yet");
	Report* report=new Report();
    double limit=1e-50; //1e-17		
	for(int i=1;i<=p;i++)
		{
			if(!(abs(h(i))<limit))
				report->mKey[i-1]=h(i);
			else
				report->mKey[i-1]=0;
		}                        
    report->mNumberOfIdenticalPoints=r1;    
	if((r1*n+q)>=p)
    {
		report->mStandardDeviation=sig0;
		report->mStandardDeviationInCoordinates=(sig0/sqrt((double)(n)));
		report->mNumberOfIterations=nofc;
		report->mSuccess=true;
        }
    else    
	{
		report->mSuccess=false;
	}

	//stability report
	if(covariance.cols() != 0)	//constraint for non-composite transformation
	{
		if(gauss)
			report->mUsedSolutionAlgorithm=Report::usaGaussJordanElimination;
		else	
		{
			report->mUsedSolutionAlgorithm=Report::usaSvd;
			if(nullity != 0)
				report->mNullity=true;
		}			
		if(q!=0)
			report->mSumOfNormalEquationsAbsoluteResiduals=test;
		report->mCorrelation.mMaximal=condition;
		report->mCorrelation.mStart=max_cor.i;
		report->mCorrelation.mEnd=max_cor.j;
		report->mCorrelation.mMatrix=create_correlation_matrix();
	}
	//stability report

	//residuals
	report->mResiduals.reset(r1,n+1);
    for(int i = 0; i < r1; i++)
	{
		report->mResiduals(i+1,n+1)=(*Xi)(i+1,n+1);
		for(int j = 1; j<=n; j++)
		{
			report->mResiduals(i+1,j)=v(i*n+j);
		}
	}
	return report;
}

GNU_gama::Mat<> all_tran_base::create_correlation_matrix()const
{
	GNU_gama::Mat<> matrix;
	matrix.reset(p,p);
	double cor;
	for(int i = 1; i<=p; i++)
		{			
			for(int j = 1; j<=p; j++)	
			{
				if(	covariance(i,i)==0 || covariance(j,j)==0 )	
					if(	covariance(i,j)==0 )	cor = 0;
					else	cor = 1;					
				else	cor = covariance(i,j)/sqrt(covariance(i,i)*covariance(j,j));								
				matrix(i,j)=cor;
			}			
		}
	return matrix;
}


void all_tran_base::adjustment()
        {
        for(int i=1;i<=r1;i++)
			for(int j=1;j<=n;j++)  dX((i-1)*n+j)=(*Xi)(i,j)-X0T(i,j);
		
		//kout<<'\n'<<dX<<'\n'<<X0T<<'\n'<<(*Xi)<<endl;
		gauss = true;
		Mat<> JTPJ,JTPJi,N,Ni,s_Ni;		
		SVD<> svd;
		double s_sig0;
		Vec<> s_v,s_dh,el,dhh,s_dhh,JTPl;

		Mat<> SVD_Wi(p+q,p+q);
		SVD_Wi.set_zero();
		Vec<> w;
 		
		if(p == (r1*n+q)) //if only necessary number of points then Newton method
		{
			N.reset(p,p);
			N.set_zero();
			el.reset(p);
			for(int i=1;i<= n*r1;i++)
			{
				el(i) = dX(i);
				for(int j=1;j<=p;j++)	N(i,j) = J(i,j);
			}
			for(int i=1;i<=q;i++)
			{
				el(n*r1+i) = b(i);
				for(int j=1;j<=p;j++)	N(n*r1+i,j) = B(j,i);
			}
			try	
			{
				Ni = inv(N); 
				dhh = Ni*el;
				for(int i=1;i<=p;i++)	dh(i) = dhh(i);
				//dh = inv(N)*el;
			}								
			catch(GNU_gama::Exception::matvec&)
			{
				gauss = false;
			}
			svd.reset(N);		
			svd.decompose();			
			svd.solve(el,s_dh);		
			w = svd.SVD_W();
		}
		else
		{	
			N.reset(p+q,p+q);	
			N.set_zero();
			JTPJ = trans(J)*(P*J);
			el.reset(p+q);
			JTPl = trans(J)*(P*dX);

			for(int i=1;i<=p;i++)				
			{
				el(i) = JTPl(i);
				for(int j=1;j<=p;j++)	N(i,j) = JTPJ(i,j);
			}
			for(int i=1;i<=q;i++)				
			{
				el(p+i) = b(i);
				for(int j=1;j<=p;j++)	
				{
					N(p+i,j) = B(j,i);
					N(j,p+i) = B(j,i);
				}
			}			
			try	
			{ 
				Ni = inv(N); 
				dhh = Ni*el;
				for(int i=1;i<=p;i++)	dh(i) = dhh(i);
			}						
			catch(GNU_gama::Exception::matvec&)	
			{
				gauss = false;
			}			
			if( q == 0 )
			{
				svd.reset(P_sqrt*J);		
				svd.decompose();			
				svd.solve(P_sqrt*dX,s_dh);		
				w = svd.SVD_W();
			}
			else  
			{
				svd.reset(N);		
				svd.decompose();	
				
				w = svd.SVD_W();								
				for(int i = 1; i<=p+q; i++)	if(w(i) != 0)	SVD_Wi(i,i)=1.0/w(i);				
				s_Ni = svd.SVD_V()*SVD_Wi*trans(svd.SVD_U());				
				s_dhh = s_Ni*el;				
				//svd.solve(el,s_dhh);

				s_dh.reset(p);
				for(int i=1;i<=p;i++)	s_dh(i) = s_dhh(i);				
			}
		}				
		
		if(gauss)
		{
			v = J*dh - dX;
			sig0 = (trans(v)*(P*v));
		}
		nullity = svd.nullity();		
		s_v = J*s_dh - dX;
		s_sig0 = (trans(s_v)*(P*s_v));		
		double s_test = 0;
		//nove testovani		
		if(q == 0)
		{
			if(gauss)	if(sig0 > s_sig0)	gauss = false;						
		}
		else
		{				
			Vec<> s_test_r = N*s_dhh - el;
			for(int i = 1;i<=p+q;i++)	s_test += fabs(s_test_r(i));
			if(gauss)
			{
				Vec<> test_r;												
				test_r = N*dhh - el;				
				test = 0;				
				for(int i = 1;i<=p+q;i++)	test += fabs(test_r(i));
				if(test > s_test)	gauss = false;			
			}
		}
		//nove testovani

		//condition
		//Vec<> w = svd.SVD_W();
		int nw = w.dim();
		double min = w(1), max = w(1);
		for(int i = 2; i<=nw; i++)
		{				
			if(min > w(i)) min = w(i);
			if(max < w(i)) max = w(i);
		}
		if(min == 0) min = 1e-12;
		condition = max/min;
		//kout<<endl<<w<<endl;
		//condition

		//gauss = true;
		if(gauss)	
		{
			JTPJi.reset(p,p);
			for(int i=1;i<=p;i++)				
				for(int j=1;j<=p;j++)	JTPJi(i,j) = Ni(i,j);
			covariance = JTPJi;
		}
		else
		{			
			if( q == 0 )	
			{
				for(int i = 1; i<=p+q; i++)	if(w(i) != 0)	SVD_Wi(i,i)=1.0/(w(i)*w(i));	
				covariance = svd.SVD_V()*SVD_Wi*trans(svd.SVD_V());			
			}
			else
			{	
				JTPJi.reset(p,p);
				for(int i=1;i<=p;i++)				
					for(int j=1;j<=p;j++)	JTPJi(i,j) = s_Ni(i,j);				
				covariance = JTPJi;
			}							
			dh = s_dh;	
			sig0 = s_sig0;
			v = s_v;
			test = s_test;
		}
		max_correlation(covariance);

		//kout<<'\n'<<"Maximal correlation is: "<<max_cor.cor<<" between parameters: "<<max_cor.i<<' '<<max_cor.j;
		//kout<<'\n'<<"The condition number is: "<<condition;
		//out_correlation(kout);

		double alfa = 0.95;					
		for(int i=1;i<=p;i++)   h(i) += alfa*dh(i);		
		
		/*
		kout<<'\n'<<"svd.SVD_W(): "<<svd.SVD_W();		
		kout<<'\n'<<"double(): "<<double();
		kout<<'\n'<<"svd.tol(): "<<svd.tol();	
		*/				

#ifdef DEBUG
		kout<<'\n'<<"Used solution algorithm: ";
		if(gauss) 
		{
			kout<<"Gauss-Jordan elimination";
			kout<<'\n'<<"Maximal correlation is: "<<max_cor.cor<<" between parameters: "<<max_cor.i<<' '<<max_cor.j;
		}
		else
		{
			kout<<"SVD";
			if(nullity != 0)	kout<<'\n'<<"Coefficient matrix of project equations is ill-conditioned or singular, the number of matrix defect is: "<<nullity;
			else
			{
				kout<<'\n'<<"Maximal correlation is: "<<max_cor.cor<<" between parameters: "<<max_cor.i<<' '<<max_cor.j;
				kout<<'\n'<<"The condition number of project matrix is: "<<condition;
			}
		}
#endif			
		if( (r1*n+q-p)>0 ) sig0 = sqrt(sig0/(r1+(double)(q-p)/n));	//r1*n+q-p
		else    sig0=sqrt(sig0);

        //control
		double sig0cont = 0;
		if( !(tt == "projective_double_inner" || tt == "projective_double") )
		{
			is_solved = true;
			Mat<> XT = transform_points(*xi);	
			is_solved = false;
			//kout<<endl<<XT;								
			for(int i=1;i<=r1;i++)
				for(int j=1;j<=n;j++)	sig0cont+=( (XT(i,j)-(*Xi)(i,j))*(XT(i,j)-(*Xi)(i,j)));			
			if((r1*n+q-p)>0) sig0cont = sqrt(sig0cont/(r1+(double)(q-p)/n));
			else    sig0cont=sqrt(sig0cont);
		}
		//control		

	#ifdef ITERATIONS
		if(gauss)	kout<<'\n'<<"GJA"; else	kout<<'\n'<<"SVD";
        kout<<endl;
		kout.setf(ios_base::fixed);		
        kout.width(3);kout<<nofc<<" ";        
        kout.width(15);kout<<sig0<<" ";
        kout.width(15);kout<<sig0cont;		
        kout.unsetf(ios_base::fixed);
		for(int i=1;i<=p;i++)   {kout<<" ";kout.width(15);kout<<h(i);}				
	#endif		
}



//------------------------------------------------------------------------------------------------------------------------------------------------------
void read_identical_points(Mat<>& Xi,Mat<>& xi,ifstream& ink, string tt)
{
	string a;
	/*ink>>a;
	if(a != "global_points")
		throw at_exception("all_tran_base::read_identical_points","The file with identical points isn't correct");*/
	
	//analysis Xi
	long position=ink.tellg();
	double d;
	char c;
	int cols = 0, rows = 0;
	while(ink>>d)
	{
		cols++;		
		do ink.get(c);
		while	(c == ' ' || c == '\t');
		if(c == '\n') break;
		ink.putback(c);		
	}
	while(ink>>d)	rows++;
	if(rows%cols != 0)
		throw at_exception("all_tran_base::read_identical_points - analysis Xi","The file with identical points isn't correct");
	rows = rows/cols + 1;	
	
	//read Xi	
	Xi.reset(rows,cols);
	ink.clear();
	ink.seekg(position);
	long control = ink.tellg();
    for(int i=1;i<=rows;i++)
	{		
		ink>>Xi(i,cols);
		for(int j=1; j < cols;j++)	ink>>Xi(i,j);					
	}
	
	//xi
	ink>>a;
	if(a != "local_points")
		throw at_exception("all_tran_base::read_identical_points - xi","The file with identical points isn't correct");

	//analysis xi
	position=ink.tellg();	
	int colss = 0, rowss = 0;
	while(ink>>d)
	{
		colss++;
		do ink.get(c);
		while	(c == ' ' || c == '\t');
		if(c == '\n') break;
		ink.putback(c);		
	}
	while(ink>>d)	rowss++;
	if(rowss%colss != 0)
		throw at_exception("all_tran_base::read_identical_points - analysis xi","The file with identical points isn't correct");
	rowss = rowss/colss + 1;	

	//test Xi and xi
	if(rows != rowss) // || cols != colss) for "dlt" is necessary to have diferent number of colums
		if( !(tt == "projective_double_inner" || tt == "projective_double") )
			throw at_exception("all_tran_base::read_identical_points - test Xi and xi","The matrixes Xi and xi haven't same size");

	//read xi	
	xi.reset(rowss,colss);
	ink.clear();
	ink.seekg(position);	
    for(int i=1;i<=rowss;i++)
	{
		ink>>xi(i,colss);;
		for(int j=1;j<colss;j++)	ink>>xi(i,j);
	}

	//test Xi and xi point numbers
	if( !(tt == "projective_double_inner" || tt == "projective_double") )
	{
		int different = 0;
		for(int i = 1; i<=rows; i++)	if( Xi(i,cols) != xi(i,colss) )		different++;
		if(different)		
				throw at_exception("all_tran_base::read_identical_points - test Xi and xi point numbers","The point numbers in Xi and xi are different");
	}
}

void read_points(Mat<>& x,ifstream& inp) 
{
	//analysis x
	long position=inp.tellg();
	double d;
	char c;
	int cols = 0, rows = 0;
	while(inp>>d)
	{
		cols++;
		do inp.get(c);
		while	(c == ' ' || c == '\t');
		if(c == '\n') break;
		inp.putback(c);		
	}
	while(inp>>d)	rows++;
	if(rows%cols != 0)
		throw at_exception("all_tran_base::read_points - analysis x","The file with local points isn't correct");
	rows = rows/cols + 1;
	
	//read x	
	x.reset(rows,cols);
	inp.clear();
	inp.seekg(position);	
    for(int i=1;i<=rows;i++)
	{
		inp>>x(i,cols); //in last column are point numbers
		for(int j=1;j<cols;j++)	inp>>x(i,j);
	}
}

void read_vector(Vec<>& v,ifstream& inp)
{
	//analysis v	
	long position=inp.tellg();
	double d;	
	int n = 0;
	while(inp>>d)	n++;

	//read v	
	v.reset(n);
	inp.clear();
	inp.seekg(position);	
	for(int i=1;i<=n;i++)	inp>>v(i);	
}

int sign(double x)
{
	if (x>=0) return 1;
	return (-1);
}

double e_norm (const Vec<>& v) 
{
	double e_norm = 0;
	int s = v.dim();
	for(int i = 1; i <= s; i++)	e_norm += v(i)*v(i);
	return sqrt(e_norm);
}

Vec<> cross_product (const Vec<>& v1, const Vec<>& v2) 
{
	Vec<> vv(3);
	vv(1) = v1(2)*v2(3)-v2(2)*v1(3);
	vv(2) = v1(3)*v2(1)-v1(1)*v2(3);
	vv(3) = v1(1)*v2(2)-v2(1)*v1(2);
	return vv;
}

double determinant(const Mat<>& xx)
{	if (xx.rows() != xx.cols())
		throw at_exception("all_tran_base::determinant","BadRank");   
    int radku=xx.rows();   //int sloupcu=xx.cols();
    double det = 0;
    int zn, j;
	if (radku > 1)
	{
		Mat<> subdet(radku - 1,radku - 1);
        int sco=subdet.cols();
		zn = 1;
		for (int y = 0; y < radku; y++)
        {	j = 0;
			for (int i = 0; i < radku; i++)
                if (i != y)
                    for (int x = 1; x < radku; x++) {subdet( ((int)(j+1)/sco)+1, (j+1)- ((int)((j+1)/sco)) *sco )= xx(i+1,x+1);j++;}
            det += zn * xx(y+1,1) * determinant(subdet);
            zn*=-1;
        }
	}
    else det = xx(1,1);        
    return det;
}

double bearing(double xc,double yc,double xo, double yo)
{
	if((xo-xc)==0)
	{
		if(yo>=yc)	return M_PI/2;
		else		return -(M_PI/2);
	}
	double help=atan(abs(yo-yc)/abs((xo-xc)));
	if((yo-yc)>=0&&(xo-xc)>0)       return  help;
	if((yo-yc)>=0&&(xo-xc)<0)       return  (M_PI-help);
	if((yo-yc)<=0&&(xo-xc)<0)       return  (M_PI+help);
	if((yo-yc)<=0&&(xo-xc)>0)       return  -(help);
	return 0;
}

double distance(double xc,double yc,double xo, double yo)
{	
	return sqrt((xc-xo)*(xc-xo)+(yc-yo)*(yc-yo));
}

}        // namespace 

#endif
