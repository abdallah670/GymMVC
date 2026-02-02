import os
import requests
import google.generativeai as genai
import sys

def generate_post():
    # Setup Gemini
    api_key = os.getenv("GEMINI_API_KEY")
    if not api_key:
        print("Error: GEMINI_API_KEY not found")
        sys.exit(1)
        
    genai.configure(api_key=api_key)
    model = genai.GenerativeModel('gemini-pro')
    
    prompt = """
    You are a Senior Full-Stack Developer and technical thought leader.
    Your goal is to write a high-value, educational LinkedIn post about a deep-dive topic in ASP.NET Core or Angular.
    
    Topics can include: 
    - Middleware, Dependency Injection, Entity Framework performance, Minimal APIs.
    - RxJS, Angular Signals, Component lifecycles, Modern state management.
    
    Requirements:
    1. Start with a catchy headline.
    2. Use bullet points for readability.
    3. Include 2-3 specific technical "pro-tips".
    4. Keep the tone professional, helpful, and expert.
    5. Add 5-7 relevant hashtags at the bottom.
    6. Return ONLY the text of the post.
    
    Write a post about a randomly selected advanced feature of ASP.NET Core or Angular (alternating).
    """
    
    response = model.generate_content(prompt)
    return response.text.strip()

def post_to_linkedin(content):
    access_token = os.getenv("LINKEDIN_ACCESS_TOKEN")
    person_id = os.getenv("LINKEDIN_PERSON_ID") # e.g., "urn:li:person:abcdefg"

    if not access_token or not person_id:
        print("Error: LinkedIn credentials not found. Check LINKEDIN_ACCESS_TOKEN and LINKEDIN_PERSON_ID.")
        # For testing, we print and don't exit if we just want to see the AI output
        print("\n--- GENERATED CONTENT (NOT POSTED) ---\n")
        print(content)
        return

    url = "https://api.linkedin.com/v2/ugcPosts"
    headers = {
        "Authorization": f"Bearer {access_token}",
        "X-Restli-Protocol-Version": "2.0.0",
        "Content-Type": "application/json"
    }

    payload = {
        "author": person_id,
        "lifecycleState": "PUBLISHED",
        "specificContent": {
            "com.linkedin.ugc.ShareContent": {
                "shareCommentary": {
                    "text": content
                },
                "shareMediaCategory": "NONE"
            }
        },
        "visibility": {
            "com.linkedin.ugc.MemberNetworkVisibility": "PUBLIC"
        }
    }

    response = requests.post(url, headers=headers, json=payload)
    
    if response.status_code == 201:
        print("Successfully posted to LinkedIn!")
    else:
        print(f"Failed to post: {response.status_code}")
        print(response.text)

if __name__ == "__main__":
    print("Generating post with Gemini...")
    content = generate_post()
    print("Post generated successfully.")
    post_to_linkedin(content)
