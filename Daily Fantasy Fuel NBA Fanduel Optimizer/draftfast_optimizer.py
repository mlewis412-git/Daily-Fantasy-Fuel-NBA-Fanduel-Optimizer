import json
from draftfast import rules
from draftfast.optimize import run
from draftfast.orm import Player

class CustomPlayer(Player):
    def __repr__(self):
        # Explicitly format cost as an integer in the representation
        return f"[{self.pos}] {self.name} ({self.team}): Cost={int(self.cost)}, Proj={self.proj}"


json_file_path = 'C:\\Users\\Administrator\\source\\repos\\Daily Fantasy Fuel NBA Fanduel Optimizer\\Daily Fantasy Fuel NBA Fanduel Optimizer\\player_pool.json'

def generate_roster_from_json_file(file_path):
    with open(file_path, 'r') as file:
        player_data = json.load(file)
    
    # Debug print the structure of the first player in the JSON data
    print("First player in JSON:", player_data[0])

    player_pool = [
        CustomPlayer(
        name=player.get('NAME'),  # Changed to 'NAME' to match the JSON structure
        cost=int(player.get('salary', 0)),  # Convert 'salary' to an int and scale appropriately
        proj=float(player.get('proj', 0)),  # Changed to 'proj' to match the JSON structure
        pos='/'.join(player.get('positions', [])),  # Join positions list into a single string
        team=player.get('team', 'NO TEAM')  # Changed to 'team' to match the JSON structure
    )
        
        for player in player_data
    ]

    # Debug print to confirm correct data retrieval
    for player in player_pool[:5]:  # Print first 5 players for brevity
        print(player)

    roster = run(
        rule_set=rules.FD_NBA_RULE_SET,
        player_pool=player_pool,
        verbose=True,
    )
    
    return roster

if __name__ == '__main__':
    optimized_roster = generate_roster_from_json_file(json_file_path)
    print(optimized_roster)
