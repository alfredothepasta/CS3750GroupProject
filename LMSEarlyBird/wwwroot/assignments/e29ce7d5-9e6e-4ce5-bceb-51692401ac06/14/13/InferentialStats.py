import pandas as pd
from scipy import stats
import numpy as np
from sklearn.calibration import LabelEncoder
import statsmodels.api as sm
from statsmodels.formula.api import ols
import plotly.express as px

from sklearn.feature_selection import RFE
from sklearn.linear_model import LogisticRegression
from sklearn.feature_selection import SelectKBest
from sklearn.feature_selection import chi2



def printTitle(title):
    print()
    print('-'*(len(title) + 2))
    print(title)
    print('-'*(len(title) + 2))
    print()

def perform_chi_squared_test(df, column1, column2):
    # Create a contingency table of the two specified columns
    contingency_table = pd.crosstab(df[column1], df[column2])

    # Perform the Chi-Squared test
    chi2, p, dof, expected = stats.chi2_contingency(contingency_table)
    rounded_expected = np.round(expected, 2)

    # Print the results and analysis statement
    print(f"Chi-Squared Test Results {column1} and {column2}:")
    print(f"Chi-Squared Statistic: {chi2:.3g}")
    print(f"Expected Frequencies: {rounded_expected}")
    print(f"Degrees of Freedom: {dof}")
    print(f"P-value: {p:.3g}")
    if p < 0.05:
        print(f"These calculations suggest that there is a significant association between {column1} and {column2}. The low p-value tells us to reject the null hypothesis that there is no association between {column1} and {column2}.")
    else:
        print(f"The p-value suggests that there is no significant association between {column1} and {column2}. We fail to reject the null hypothesis that there is no association between {column1} and {column2}.")
    print()
    
# Read the CSV file into a DataFrame
df = pd.read_csv('Titanic Crew.csv')

# Drop the URL column
df = df.drop(columns=['URL'])

# Filter out people who do not have a 'LOST' or 'SURVIVED' entry in the 'Survived?' column
df = df[df['Survived?'].isin(['LOST', 'SAVED'])]

# # Write the filtered DataFrame to a new CSV file
# df.to_csv('Filtered_Titanic_Crew.csv', index=False)


#----------------------------------
# Chi-Squared Tests
#----------------------------------
printTitle("Chi-Squared Tests")

perform_chi_squared_test(df, 'Gender', 'Survived?')
perform_chi_squared_test(df, 'Class/Dept', 'Survived?')
perform_chi_squared_test(df, 'Joined', 'Survived?')


#----------------------------------
# ANOVA 
#----------------------------------
printTitle("ANOVA")

# Rename collumn to remove forward slash
df.rename(columns={'Class/Dept': 'ClassDept'}, inplace=True)

# Perform the ANOVA
lm = ols('Age ~ C(ClassDept)', data=df).fit()
table = sm.stats.anova_lm(lm, typ=2)

# Print the ANOVA results
print("ANOVA Results for Class/Dept and Age:")
print(table)

# If there is statistical significance, perform Tukey HSD

from statsmodels.stats.multicomp import pairwise_tukeyhsd
tukey = pairwise_tukeyhsd(df['Age'], df['ClassDept'])
print()
print("Tukey HSD Results:")
print(tukey.summary())
print()
print("The ANOVA results indicate a significant association between Class/Dept and Age.")
print("The Tukey HSD report shows that these departments are are statistically significant from each other:")
print(" - Deck Crew & Restraunt Staff")
print(" - Deck Crew Titanic Officers & Restraunt Staff")
print(" - Engineering Crew & Restraunt Staff")
print(" - Victualling Crew & Restraunt Staff")
print(" - Victualling Crew Postal Clerk  & Restraunt Staff")
print()
print("This tells us that the distribution of ages in the Restraunt Staff department is different from the rest of the departments.")

#----------------------------------
# Separate 'Gender' Column
#----------------------------------
printTitle("Separate 'Gender' Column")
# FEMALE
pearson_corr_gender = df['Gender'].map({'Female': 1, 'Male': 0}).corr(df['Survived?'].map({'LOST': 0, 'SAVED': 1}), method='pearson')

# Calculating Spearman's correlation
spearman_corr_gender = df['Gender'].map({'Female': 1, 'Male': 0}).corr(df['Survived?'].map({'LOST': 0, 'SAVED': 1}), method='spearman')

print(f"Pearson's correlation of 'Female' to 'Survived': {pearson_corr_gender:.3g}")
print(f"Spearman's correlation of 'Female' to 'Survived': {spearman_corr_gender:.3g}")

pearson_corr_gender = df['Gender'].map({'Female': 1, 'Male': 0}).corr(df['Survived?'].map({'LOST': 0, 'SAVED': 1}), method='pearson')

# MALE 
pearson_corr_gender = df['Gender'].map({'Female': 0, 'Male': 1}).corr(df['Survived?'].map({'LOST': 0, 'SAVED': 1}), method='pearson')
# Calculating Spearman's correlation
spearman_corr_gender = df['Gender'].map({'Female': 0, 'Male': 1}).corr(df['Survived?'].map({'LOST': 0, 'SAVED': 1}), method='spearman')

print()
print(f"Pearson's correlation of 'Male' to 'Survived': {pearson_corr_gender:.3g}")
print(f"Spearman's correlation of 'Male' to 'Survived': {spearman_corr_gender:.3g}")
print()

#----------------------------------
# Bivariate Visualizations:
#----------------------------------
#----1
# Create a scatter plot to visualize the relationship between passenger age and passenger class
fig = px.scatter(df, x='Age', y='Survived?', 
                 title='Passenger Age vs Survival Status', labels={'Age': 'Age','Survived?': 'Survival Status'})
fig.show()

#----2
# Create a bivariate bar chart to visualize the count of survivors for each passenger class
survivors_by_class = df[df['Survived?'] == 'SAVED'].groupby('ClassDept').size().reset_index(name='survivor_count')

fig = px.bar(survivors_by_class, x='ClassDept', y='survivor_count', 
             title='Count of Survivors by Passenger Dept', labels={'ClassDept': 'Passenger Dept', 'survivor_count': 'Survivor Count'})
fig.show()

#----3
# Count the number of survivors for each fare
survivors_by_fare = df[df['Survived?'] == 'SAVED'].groupby('Nationality').size().reset_index(name='survivor_count')

# Create a bivariate scatter plot to visualize the number of survivors by fare
fig = px.bar(survivors_by_fare, x='Nationality', y='survivor_count', 
                 title='Number of Survivors by Nationality', labels={'Nationality': 'Nationality', 'survivor_count': 'Survivor Count'})
fig.show()


#----------------------------------
# Multivariate Visualizations:
#----------------------------------
# Create a bivariate bar chart to visualize the count of survivors and non-survivors for different categories within another column
fig = px.bar(df, x='ClassDept', color='Survived?', 
             title='Count of Survivors and Non-Survivors by Passenger Dept', 
             labels={'ClassDept': 'Passenger Dept', 'Survived?': 'Survival Status'})
fig.show()

#----2
fig = px.bar(df, x='Gender', color='Survived?', 
             title='Count of Survivors and Non-Survivors by Gender', 
             labels={'Gender': 'Gender', 'Survived?': 'Survival Status'})
fig.show()


#----------------------------------
# Part 1:
#----------------------------------
printTitle("Part 1")

print("My top features:")
print(" - Gender")
print(" - Department")
print("My reasoning is that the proportions of these two features do not match the proportions of people who survived and did not survive. This tells me that there is some realtion to Survival and Gender and survival and passenger department")


#----------------------------------
# Part 2:
#----------------------------------
printTitle("Part 2")

cleaned_data = df.drop(['Name', 'Born', 'Died', 'Fare','Cabin','Ticket','Boat','Body'], axis=1)

le = LabelEncoder()
categorical_cols = ['ClassDept','Joined', 'Occupation', 'Survived?', 'Nationality', 'Gender']
for col in categorical_cols:
    cleaned_data[col] = le.fit_transform(cleaned_data[col])

y = cleaned_data['Survived?']
X = cleaned_data.drop('Survived?', axis=1)

# Initialize the logistic regression model
model = LogisticRegression(max_iter=10000)
# Initialize the RFE selector with 5 features to select
rfe = RFE(model, n_features_to_select = 3)
# Fit the RFE selector to the data
rfe.fit(X, y)

print("RFE reported these top 3 features:")
print(" - Gender")
print(" - Department")
print(" - Joined")

X = cleaned_data.drop(['Survived?','Occupation'], axis=1)

# Initialize the SelectKBest selector with chi-squared test to select 3 features
selector = SelectKBest(score_func=chi2, k=3)

# Fit the selector to the data
X_new = selector.fit_transform(X, y)

# Get the indices of the selected features
selected_features_indices = selector.get_support(indices=True)

# Get the feature names corresponding to the selected indices
selected_features_names = X.columns[selected_features_indices]

# Print the names of the selected features
print("Select K best selected these top 3 features:")
for feature in selected_features_names:
    print(" -", feature)
    
#----------------------------------
# Part 3:
#----------------------------------
printTitle("Part 3")
print("Based on the analysis of the data above I would select Gender and Department as the best predicting features of Survival for this dataset.")