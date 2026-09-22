import time, pygame

# 실제 프로젝트에서 메인 게임 루프만 발췌한 코드입니다.
#
# screen.main(): 입력과 게임 로직 처리
# screen.render_main(): 화면 렌더링
#
# screens: 모든 화면 클래스를 키로 가지는 딕셔너리
# global_data: 모든 화면에서 ㅉ존재하는 데이터
# gamepad, surface: pygame 화면을 그릴 때 필요한 요소


# 입력 및 게임 로직과 렌더링 주기를 분리한 메인 게임 루프
def run_game():
    game_init() # pygame과 각 화면 클래스 초기화

    screen = screens[global_data.screen_type]
    render_time = 0
    input_time = time.perf_counter()
    while True:
        now = time.perf_counter()
        previous_screen_type = global_data.screen_type

        global_data.clock = now - input_time
        screen.main(global_data)
        input_time = now

        # 화면 전환 시 처음 한 번만 실행하는 코드 실행
        if global_data.screen_type != previous_screen_type:
            if global_data.screen_type not in screens:
                write_log('screen = %s' %screen)
                break

            if global_data.screen_type == 'end':
                break

            screen = screens[global_data.screen_type]
            screen.first_func()

        # 렌더링은 설정의 최대 FPS에 맞춰 진행
        render_interval = 1 / global_data.max_frame_rate
        if now - render_time > render_interval:
            global_data.clock = now - render_time
            screen.render_main(global_data)

            check_change_screen_size()
            gamepad.blit(surface, (0, 0))
            pygame.display.update()

            render_time = now

        # CPU 사용량을 고려하여 대기 시간 추가
        time.sleep(0.0005)

    pygame.quit()