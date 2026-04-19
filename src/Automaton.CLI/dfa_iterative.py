from enum import Enum, auto

# Definícia stavov
class State(Enum):
    q0 = auto()
    q1 = auto()
    q2 = auto()
    q3 = auto()

class DFA:
    def __init__(self, transition_table : dict, accepting_states: set, init_state: State):
        self.transition_table = transition_table
        self.accepting_states = accepting_states
        self.init_state = init_state
        self.current_state = None

    def check(self, string: str) -> bool:
        self.current_state = self.init_state
        for symbol in string:
            if (self.current_state, symbol) in self.transition_table:
                print(f"actual state={self.current_state}")
                self.current_state = self.transition_table[(self.current_state, symbol)]
            else:
                return False
        return self.current_state in accepting_states

if __name__ == "__main__":
    # Tabuľka prechodovej funkcie
    transition_table = {
        (State.q0, '0'): State.q1,
        (State.q0, '1'): State.q2,
        (State.q2, '0'): State.q2,
        (State.q2, '1'): State.q2,
        (State.q1, '0'): State.q3,
        (State.q1, '1'): State.q3,
        (State.q3, '0'): State.q3,
        (State.q3, '1'): State.q3
    }

    # Množina akceptačných stavov
    accepting_states = {State.q1,State.q2}

    # DKA akceptujúci jazyk binárnych numerálov
    dfa = DFA(transition_table=transition_table,
              accepting_states=accepting_states,
              init_state=State.q0)

    input_string = input ("Zadaj vstupný reťazec> ")
    while input_string != "quit":
        print(f"Reťazec '{ input_string }' " 
              f"{'JE ' if dfa.check(input_string) else 'NIE JE '}akceptovaný!"
        )
        input_string = input ("Zadaj vstupný reťazec> ")